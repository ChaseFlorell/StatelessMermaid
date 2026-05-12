using System.Threading.Tasks;
using FluentAssertions;
using Stateless;
using StatelessMermaid.Tests.Utilities;
using Xunit;
using System.ComponentModel;

namespace StatelessMermaid.Tests;

public class LabelFormatTests
{
    // ── State labels ────────────────────────────────────────────────────────

    [Fact]
    public async Task GivenHumanReadableLabelFormat_WhenStateNameIsPascalCase_ThenStateLabelIsSplit()
    {
        var machine = new StateMachine<State, Trigger>(State.WaitingForInput);
        machine.Configure(State.WaitingForInput).Permit(Trigger.ProcessData, State.ProcessingComplete);
        machine.Configure(State.ProcessingComplete);

        var act = () => machine.ToMermaid(new MermaidOptions { LabelFormat = LabelFormat.HumanReadable });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    [Fact]
    public async Task GivenHumanReadableLabelFormat_WhenStateHasDescriptionAttribute_ThenDescriptionTakesPrecedence()
    {
        var machine = new StateMachine<DescribedState, Trigger>(DescribedState.ActiveSession);
        machine.Configure(DescribedState.ActiveSession).Permit(Trigger.ProcessData, DescribedState.IdleMode);
        machine.Configure(DescribedState.IdleMode);

        var act = () => machine.ToMermaid(new MermaidOptions { LabelFormat = LabelFormat.HumanReadable });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    // ── Trigger labels ──────────────────────────────────────────────────────

    [Fact]
    public async Task GivenHumanReadableLabelFormat_WhenTriggerIsIgnored_ThenIgnoredTriggerLabelIsSplit()
    {
        var machine = new StateMachine<State, Trigger>(State.WaitingForInput);
        machine.Configure(State.WaitingForInput)
            .Ignore(Trigger.ProcessData)
            .Permit(Trigger.GoToIdle, State.ProcessingComplete);
        machine.Configure(State.ProcessingComplete);

        var act = () => machine.ToMermaid(new MermaidOptions { LabelFormat = LabelFormat.HumanReadable });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    // ── Action method names in notes ────────────────────────────────────────

    [Fact]
    public async Task GivenHumanReadableLabelFormat_WhenEntryAndExitActionsAreRegistered_ThenMethodNamesAreSplit()
    {
        var machine = new StateMachine<State, Trigger>(State.WaitingForInput);
        machine.Configure(State.WaitingForInput)
            .OnEntry(OnEnterWaitingForInput)
            .OnExit(OnExitWaitingForInput)
            .Permit(Trigger.ProcessData, State.ProcessingComplete);
        machine.Configure(State.ProcessingComplete)
            .OnActivate(OnActivateProcessingComplete)
            .OnDeactivate(OnDeactivateProcessingComplete);

        var act = () => machine.ToMermaid(new MermaidOptions { LabelFormat = LabelFormat.HumanReadable });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    // ── Acronym handling ────────────────────────────────────────────────────

    [Fact]
    public void GivenHumanReadableLabelFormat_WhenIdentifierContainsAcronym_ThenAcronymIsKeptTogether()
    {
        var machine = new StateMachine<AcronymState, AcronymTrigger>(AcronymState.HTTPClientReady);
        machine.Configure(AcronymState.HTTPClientReady).Permit(AcronymTrigger.SendHTTPRequest, AcronymState.ParseURLResponse);
        machine.Configure(AcronymState.ParseURLResponse);

        var diagram = machine.ToMermaid(new MermaidOptions
        {
            IncludeMarkdownBlocks = false,
            LabelFormat = LabelFormat.HumanReadable
        });

        diagram.Should().Contain("HTTP Client Ready");
        diagram.Should().Contain("Parse URL Response");
        diagram.Should().Contain("Send HTTP Request");
    }

    // ── Raw (default) ───────────────────────────────────────────────────────

    [Fact]
    public async Task GivenRawLabelFormat_WhenGeneratingDiagram_ThenIdentifiersAreUnchanged()
    {
        var machine = new StateMachine<State, Trigger>(State.WaitingForInput);
        machine.Configure(State.WaitingForInput)
            .OnEntry(OnEnterWaitingForInput)
            .Permit(Trigger.ProcessData, State.ProcessingComplete);
        machine.Configure(State.ProcessingComplete);

        var act = () => machine.ToMermaid(new MermaidOptions { LabelFormat = LabelFormat.Raw });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    private enum State { WaitingForInput, ProcessingComplete, GoToIdle }
    private enum Trigger { ProcessData, GoToIdle }
    private enum AcronymState { HTTPClientReady, ParseURLResponse }
    private enum AcronymTrigger { SendHTTPRequest }

    private enum DescribedState
    {
        [Description("Active User Session")] ActiveSession,
        [Description("System Idle")] IdleMode,
    }

    private static void OnEnterWaitingForInput() { }
    private static void OnExitWaitingForInput() { }
    private static void OnActivateProcessingComplete() { }
    private static void OnDeactivateProcessingComplete() { }
}
