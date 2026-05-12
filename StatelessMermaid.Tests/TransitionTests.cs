using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Stateless;
using Stateless.Reflection;
using StatelessMermaid.Tests.Utilities;
using Xunit;

namespace StatelessMermaid.Tests;

public class TransitionTests
{
    [Fact]
    public async Task GivenInternalTransition_WhenGeneratingDiagram_ThenTransitionIsLabeledWithInternalPrefix()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.A);
        machine.Configure(State.A)
            .InternalTransition(Trigger.Ping, _ => { })
            .Permit(Trigger.Go, State.B);
        machine.Configure(State.B)
            .Permit(Trigger.Back, State.A);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenDynamicTransition_WhenGeneratingDiagram_ThenChoiceNodeIsCreated()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.A);
        machine.Configure(State.A)
            .PermitDynamic(
                Trigger.Route,
                () => State.B,
                "decide",
                new DynamicStateInfos
                {
                    { State.B, "condition true" },
                    { State.C, "condition false" }
                });
        machine.Configure(State.B);
        machine.Configure(State.C);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenMultiplePermitIfForSameTrigger_WhenGeneratingDiagram_ThenChoiceNodeIsCreated()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.A);
        machine.Configure(State.A)
            .PermitIf(Trigger.Go, State.B, () => true, "guard B")
            .PermitIf(Trigger.Go, State.C, () => false, "guard C");
        machine.Configure(State.B);
        machine.Configure(State.C);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenParameterizedTriggerWithPrimitiveType_WhenGeneratingDiagram_ThenTypeAnnotationAppearsOnTransitionLabel()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.A);
        var paramTrigger = machine.SetTriggerParameters<int>(Trigger.Go);
        machine.Configure(State.A)
            .PermitIf(paramTrigger, State.B, n => n > 0);
        machine.Configure(State.B);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenParameterizedTriggerWithMultipleTypes_WhenGeneratingDiagram_ThenAllTypeAnnotationsAppearOnTransitionLabel()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.A);
        var paramTrigger = machine.SetTriggerParameters<int, string>(Trigger.Go);
        machine.Configure(State.A)
            .PermitIf(paramTrigger, State.B, (n, _) => n > 0);
        machine.Configure(State.B);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenParameterizedTriggerWithBoolType_WhenGeneratingDiagram_ThenFriendlyTypeNameIsUsedInAnnotation()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.A);
        var paramTrigger = machine.SetTriggerParameters<bool>(Trigger.Go);
        machine.Configure(State.A)
            .PermitIf(paramTrigger, State.B, flag => flag);
        machine.Configure(State.B);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenTerminalState_WhenGeneratingDiagram_ThenEndMarkerIsAppended()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.A);
        machine.Configure(State.A).Permit(Trigger.Go, State.B);
        machine.Configure(State.B);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenStateWithOnlyInternalTransitions_WhenGeneratingDiagram_ThenStateIsConsideredTerminal()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.A);
        machine.Configure(State.A).Permit(Trigger.Go, State.B);
        machine.Configure(State.B)
            .InternalTransition(Trigger.Ping, _ => { });

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public void GivenParameterizedTriggersWithRemainingPrimitiveTypes_WhenGeneratingDiagram_ThenFriendlyTypeNamesAppearInLabels()
    {
        var cases = new (Type type, string alias)[]
        {
            (typeof(byte), "byte"),
            (typeof(sbyte), "sbyte"),
            (typeof(short), "short"),
            (typeof(ushort), "ushort"),
            (typeof(uint), "uint"),
            (typeof(long), "long"),
            (typeof(ulong), "ulong"),
            (typeof(float), "float"),
            (typeof(double), "double"),
            (typeof(decimal), "decimal"),
            (typeof(char), "char"),
            (typeof(object), "object"),
        };

        var setMethod = typeof(StateMachine<State, Trigger>)
            .GetMethods()
            .Single(m => m.Name == "SetTriggerParameters" && m.GetGenericArguments().Length == 1);

        foreach (var (type, alias) in cases)
        {
            var machine = new StateMachine<State, Trigger>(State.A);
            setMethod.MakeGenericMethod(type).Invoke(machine, [Trigger.Go]);
            machine.Configure(State.A).Permit(Trigger.Go, State.B);
            machine.Configure(State.B);

            var diagram = machine.ToMermaid(new MermaidOptions { IncludeMarkdownBlocks = false });
            diagram.Should().Contain($"Go({alias})", because: $"type {type.Name} should map to alias '{alias}'");
        }
    }

    [Fact]
    public void GivenParameterizedTriggerWithGenericType_WhenGeneratingDiagram_ThenFormattedTypeNameAppearsInLabel()
    {
        var machine = new StateMachine<State, Trigger>(State.A);
        var setMethod = typeof(StateMachine<State, Trigger>)
            .GetMethods()
            .Single(m => m.Name == "SetTriggerParameters" && m.GetGenericArguments().Length == 1)
            .MakeGenericMethod(typeof(List<int>));
        setMethod.Invoke(machine, [Trigger.Go]);
        machine.Configure(State.A).Permit(Trigger.Go, State.B);
        machine.Configure(State.B);

        var diagram = machine.ToMermaid(new MermaidOptions { IncludeMarkdownBlocks = false });
        diagram.Should().Contain("Go(List<int>)");
    }

    [Fact]
    public async Task GivenDynamicTransitionWithNoDestinationCriterion_WhenGeneratingDiagram_ThenChoiceArrowHasNoLabel()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.A);
        machine.Configure(State.A)
            .PermitDynamic(
                Trigger.Route,
                () => State.B,
                "decide",
                new DynamicStateInfos
                {
                    { State.B, "" },
                    { State.C, "condition false" }
                });
        machine.Configure(State.B);
        machine.Configure(State.C);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    private enum State { A, B, C }
    private enum Trigger { Go, Back, Ping, Route }
}
