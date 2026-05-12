using System.Threading.Tasks;
using FluentAssertions;
using Stateless;
using StatelessMermaid.Tests.Utilities;
using Xunit;

namespace StatelessMermaid.Tests;

public class NoteTests
{
    [Fact]
    public async Task GivenStateWithEntryAction_WhenGeneratingDiagram_ThenEntryActionAppearsInNote()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Alpha);
        machine.Configure(State.Alpha)
            .OnEntry(OnEnterAlpha)
            .Permit(Trigger.Next, State.Beta);
        machine.Configure(State.Beta);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenStateWithExitAction_WhenGeneratingDiagram_ThenExitActionAppearsInNote()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Alpha);
        machine.Configure(State.Alpha)
            .OnExit(OnExitAlpha)
            .Permit(Trigger.Next, State.Beta);
        machine.Configure(State.Beta);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenStateWithActivateAndDeactivateActions_WhenGeneratingDiagram_ThenActionsAppearInNote()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Alpha);
        machine.Configure(State.Alpha)
            .Permit(Trigger.Next, State.Beta);
        machine.Configure(State.Beta)
            .OnActivate(OnActivateBeta)
            .OnDeactivate(OnDeactivateBeta);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenIgnoredTrigger_WhenGeneratingDiagram_ThenIgnoredTriggerAppearsInNote()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Alpha);
        machine.Configure(State.Alpha)
            .Ignore(Trigger.Ping)
            .Permit(Trigger.Next, State.Beta);
        machine.Configure(State.Beta);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenStateWithMultipleActionsAndIgnoredTriggers_WhenGeneratingDiagram_ThenAllAppearInNote()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Alpha);
        machine.Configure(State.Alpha)
            .OnEntry(OnEnterAlpha)
            .OnExit(OnExitAlpha)
            .Ignore(Trigger.Ping)
            .Permit(Trigger.Next, State.Beta);
        machine.Configure(State.Beta)
            .OnEntry(OnEnterBeta)
            .Permit(Trigger.Next, State.Gamma);
        machine.Configure(State.Gamma);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenCompositeStateWithEntryAction_WhenGeneratingDiagram_ThenNoteIsNotEmittedForCompositeState()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Alpha);
        machine.Configure(State.Alpha)
            .OnEntry(OnEnterAlpha);
        machine.Configure(State.Beta)
            .SubstateOf(State.Alpha)
            .OnEntry(OnEnterBeta)
            .Permit(Trigger.Next, State.Gamma);
        machine.Configure(State.Gamma);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenRightToLeftDirection_WhenGeneratingDiagram_ThenNotesArePlacedOnLeft()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Alpha);
        machine.Configure(State.Alpha)
            .OnEntry(OnEnterAlpha)
            .Permit(Trigger.Next, State.Beta);
        machine.Configure(State.Beta);

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Direction = DiagramDirection.RightToLeft });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenLeftToRightDirection_WhenGeneratingDiagram_ThenNotesArePlacedOnRight()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Alpha);
        machine.Configure(State.Alpha)
            .OnEntry(OnEnterAlpha)
            .Permit(Trigger.Next, State.Beta);
        machine.Configure(State.Beta);

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Direction = DiagramDirection.LeftToRight });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    private enum State { Alpha, Beta, Gamma }
    private enum Trigger { Next, Ping }

    private static void OnEnterAlpha() { }
    private static void OnExitAlpha() { }
    private static void OnEnterBeta() { }
    private static void OnActivateBeta() { }
    private static void OnDeactivateBeta() { }
}
