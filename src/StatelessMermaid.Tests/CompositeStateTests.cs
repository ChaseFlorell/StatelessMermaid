using System.Threading.Tasks;
using FluentAssertions;
using Stateless;
using StatelessMermaid.Tests.Utilities;
using Xunit;

namespace StatelessMermaid.Tests;

public class CompositeStateTests
{
    [Fact]
    public async Task GivenCompositeState_WhenGeneratingDiagram_ThenSubstatesAreGroupedInsideParentBlock()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Offline);
        machine.Configure(State.Offline)
            .Permit(Trigger.Connect, State.Idle);
        machine.Configure(State.Online)
            .Permit(Trigger.Disconnect, State.Offline);
        machine.Configure(State.Idle)
            .SubstateOf(State.Online)
            .Permit(Trigger.Start, State.Busy);
        machine.Configure(State.Busy)
            .SubstateOf(State.Online)
            .Permit(Trigger.Finish, State.Idle);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenSiblingTransitions_WhenGeneratingDiagram_ThenTransitionsAreRenderedInsideCompositeBlock()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Offline);
        machine.Configure(State.Offline)
            .Permit(Trigger.Connect, State.Idle);
        machine.Configure(State.Online)
            .Permit(Trigger.Disconnect, State.Offline);
        machine.Configure(State.Idle)
            .SubstateOf(State.Online)
            .Permit(Trigger.Start, State.Busy);
        machine.Configure(State.Busy)
            .SubstateOf(State.Online)
            .Permit(Trigger.Finish, State.Idle);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenTransitionToNonSibling_WhenGeneratingDiagram_ThenTransitionIsRenderedOutsideCompositeBlock()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Offline);
        machine.Configure(State.Offline)
            .Permit(Trigger.Connect, State.Idle);
        machine.Configure(State.Online);
        machine.Configure(State.Idle)
            .SubstateOf(State.Online)
            .Permit(Trigger.Start, State.Busy)
            .Permit(Trigger.Disconnect, State.Offline);
        machine.Configure(State.Busy)
            .SubstateOf(State.Online)
            .Permit(Trigger.Finish, State.Idle)
            .Permit(Trigger.Disconnect, State.Offline);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    private enum State
    {
        Offline,
        Online,
        Idle,
        Busy
    }

    private enum Trigger
    {
        Connect,
        Disconnect,
        Start,
        Finish
    }
}