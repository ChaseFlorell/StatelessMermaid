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

    [Fact]
    public async Task GivenThreeLevelCompositeState_WhenGeneratingDiagram_ThenAllLevelsAreNested()
    {
        // Arrange: Root > Mid > Leaf hierarchy
        var machine = new StateMachine<DeepState, Trigger>(DeepState.Leaf);
        machine.Configure(DeepState.Root)
            .Permit(Trigger.Disconnect, DeepState.Outside);
        machine.Configure(DeepState.Mid)
            .SubstateOf(DeepState.Root);
        machine.Configure(DeepState.Leaf)
            .SubstateOf(DeepState.Mid)
            .Permit(Trigger.Finish, DeepState.Outside);
        machine.Configure(DeepState.Outside);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenTransitionBetweenSubstatesOfDifferentParents_WhenGeneratingDiagram_ThenTransitionIsOutsideBothBlocks()
    {
        // Arrange: Two composite states each with a substate; sibling check returns false
        var machine = new StateMachine<CrossState, Trigger>(CrossState.SubA);
        machine.Configure(CrossState.GroupA);
        machine.Configure(CrossState.SubA)
            .SubstateOf(CrossState.GroupA)
            .Permit(Trigger.Go, CrossState.SubB);
        machine.Configure(CrossState.GroupB);
        machine.Configure(CrossState.SubB)
            .SubstateOf(CrossState.GroupB);

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

    private enum DeepState
    {
        Root,
        Mid,
        Leaf,
        Outside
    }

    private enum CrossState
    {
        GroupA,
        SubA,
        GroupB,
        SubB
    }

    private enum Trigger
    {
        Connect,
        Disconnect,
        Start,
        Finish,
        Go
    }
}