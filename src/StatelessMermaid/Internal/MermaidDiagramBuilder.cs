using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Stateless.Reflection;

namespace StatelessMermaid.Internal;

internal sealed class MermaidDiagramBuilder
{
    private readonly MermaidOptions _options;
    private readonly IReadOnlyDictionary<string, Type[]> _triggerParameters;
    private readonly StringBuilder _sb = new();

    internal MermaidDiagramBuilder(MermaidOptions options, IReadOnlyDictionary<string, Type[]> triggerParameters)
    {
        _options = options;
        _triggerParameters = triggerParameters;
    }

    private string TriggerLabel(string label)
    {
        if (!_triggerParameters.TryGetValue(label, out var types) || types.Length == 0)
        {
            return label;
        }

        return $"{label}({string.Join(", ", types.Select(t => t.ToFriendlyName()))})";
    }

    internal string Build(StateMachineInfo info)
    {
        WriteHeader();
        WriteInitialTransition(info);
        WriteStateDescriptions(info);
        WriteCompositeBlocks(info);
        WriteAllTransitions(info);
        WriteTerminalMarkers(info);
        WriteAllNotes(info);
        WriteFooter();
        return _sb.ToString();
    }


    private void WriteHeader()
    {
        if (_options.IncludeMarkdownBlocks)
        {
            _sb.AppendLine("```mermaid");
        }

        if (!string.IsNullOrWhiteSpace(_options.Title))
        {
            _sb.AppendLine("---");
            _sb.AppendLine($"title: {_options.Title}");
            _sb.AppendLine("---");
        }

        _sb.AppendLine($"stateDiagram{_options.Version.ToMermaidSyntax()}");
        _sb.AppendLine($"\tdirection {_options.Direction.ToMermaidSyntax()}");
    }

    private void WriteFooter()
    {
        if (_options.IncludeMarkdownBlocks)
        {
            _sb.AppendLine("```");
        }
    }


    private void WriteInitialTransition(StateMachineInfo info)
    {
        _sb.AppendLine($"\t[*] --> {StateId(info.InitialState)}");
    }


    private void WriteStateDescriptions(StateMachineInfo info)
    {
        foreach (var state in info.States)
        {
            var id = StateId(state);
            var label = GetDescription(state) ?? state.ToString();
            if (label != id)
            {
                _sb.AppendLine($"\t{id}: {label}");
            }
        }
    }

    private static string? GetDescription(StateInfo state)
    {
        if (state.UnderlyingState is not Enum enumValue)
        {
            return null;
        }

        var member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }


    private void WriteCompositeBlocks(StateMachineInfo info)
    {
        foreach (var state in info.States.Where(s => s.Superstate == null && s.Substates.Any()))
            WriteCompositeBlock(state, 1);
    }

    private void WriteCompositeBlock(StateInfo state, int indent)
    {
        var prefix = Prefix(indent);
        _sb.AppendLine($"{prefix}state {StateId(state)} {{");

        foreach (var substate in state.Substates)
        {
            if (substate.Substates.Any())
                WriteCompositeBlock(substate, indent + 1);

            WriteSiblingTransitions(substate, state, indent + 1);
        }

        _sb.AppendLine($"{prefix}}}");
    }

    private void WriteSiblingTransitions(StateInfo state, StateInfo parent, int indent)
    {
        var siblings = state.FixedTransitions.Where(t => !t.IsInternalTransition && IsSiblingOf(t.DestinationState, parent));
        WriteFixedTransitions(siblings, state, Prefix(indent));
    }


    private void WriteAllTransitions(StateMachineInfo info)
    {
        foreach (var state in info.States)
            WriteNonSiblingTransitions(state);
    }

    private void WriteNonSiblingTransitions(StateInfo state)
    {
        foreach (var t in state.FixedTransitions.Where(t => t.IsInternalTransition))
            _sb.AppendLine($"\t{StateId(state)} --> {StateId(state)}: [internal] {TriggerLabel(t.Trigger.ToString()!)}");

        var nonSiblings = state.FixedTransitions.Where(t => !t.IsInternalTransition && !IsSiblingOf(t.DestinationState, state.Superstate));
        WriteFixedTransitions(nonSiblings, state, "\t");

        foreach (var t in state.DynamicTransitions)
            WriteDynamicTransition(state, t);
    }

    // Groups transitions by trigger. A trigger with multiple destinations becomes a <<choice>> node;
    // a trigger with a single destination is a direct arrow.
    private void WriteFixedTransitions(IEnumerable<FixedTransitionInfo> transitions, StateInfo source, string prefix)
    {
        foreach (var group in transitions.GroupBy(t => t.Trigger.ToString()!))
        {
            var triggerLabel = TriggerLabel(group.Key);
            var destinations = group.ToList();

            if (destinations.Count == 1)
            {
                _sb.AppendLine($"{prefix}{StateId(source)} --> {StateId(destinations[0].DestinationState)}: {triggerLabel}");
            }
            else
            {
                var choiceId = $"{StateId(source)}_{group.Key.ToMermaidId()}_choice";
                _sb.AppendLine($"{prefix}state {choiceId} <<choice>>");
                _sb.AppendLine($"{prefix}{StateId(source)} --> {choiceId}: {triggerLabel}");
                foreach (var t in destinations)
                    _sb.AppendLine($"{prefix}{choiceId} --> {StateId(t.DestinationState)}");
            }
        }
    }

    private void WriteDynamicTransition(StateInfo state, DynamicTransitionInfo transition)
    {
        var choiceId = $"{state}_{transition.Trigger}_choice";
        _sb.AppendLine($"\tstate {choiceId} <<choice>>");
        _sb.AppendLine($"\t{state} --> {choiceId}: {TriggerLabel(transition.Trigger.ToString())}");

        foreach (var dest in transition.PossibleDestinationStates)
        {
            var label = string.IsNullOrWhiteSpace(dest.Criterion) ? string.Empty : $": {dest.Criterion}";
            _sb.AppendLine($"\t{choiceId} --> {dest.DestinationState}{label}");
        }
    }


    private void WriteTerminalMarkers(StateMachineInfo info)
    {
        foreach (var state in info.States.Where(IsTerminal))
        {
            _sb.AppendLine($"\t{state} --> [*]");
        }
    }


    private void WriteAllNotes(StateMachineInfo info)
    {
        foreach (var state in info.States.Where(s => !s.Substates.Any()))
        {
            WriteNote(state);
        }
    }

    private void WriteNote(StateInfo state)
    {
        var lines = CollectNoteLines(state);
        if (lines.Count == 0)
        {
            return;
        }

        _sb.AppendLine($"\tnote {_options.Direction.ToNotePosition()} {state}");
        foreach (var line in lines)
        {
            _sb.AppendLine($"\t\t{line}");
        }

        _sb.AppendLine($"\tend note");
    }

    private List<string> CollectNoteLines(StateInfo state)
    {
        var lines = new List<string>();
        lines.AddRange(state.EntryActions.Select(a => $"entry / {a.Method.MethodName}"));
        lines.AddRange(state.ExitActions.Select(a => $"exit / {a.MethodName}"));
        lines.AddRange(state.ActivateActions.Select(a => $"activate / {a.MethodName}"));
        lines.AddRange(state.DeactivateActions.Select(a => $"deactivate / {a.MethodName}"));
        lines.AddRange(state.IgnoredTriggers.Select(t => $"ignore: {TriggerLabel(t.Trigger.ToString()!)}"));
        return lines;
    }


    private static bool IsTerminal(StateInfo state) =>
        state.Superstate == null
        && !state.Substates.Any()
        && state.FixedTransitions.All(t => t.IsInternalTransition) && !state.DynamicTransitions.Any();

    private static bool IsSiblingOf(StateInfo dest, StateInfo? parent) =>
        parent != null
        && dest.Superstate != null
        && dest.Superstate.UnderlyingState?.Equals(parent.UnderlyingState) == true;

    private static string Prefix(int indent) => new('\t', indent);

    private static string StateId(StateInfo state) => state.ToString().ToMermaidId();
}