using Stateless;

namespace StatelessMermaid.Tests.Fixtures;

internal class DiagramOptionsStateMachineFixture : StateMachine<State, Trigger>
{
    internal DiagramOptionsStateMachineFixture() : base(State.On)
    {
        Configure(State.On).Permit(Trigger.Toggle, State.Off);
        Configure(State.Off).Permit(Trigger.Toggle, State.On);
    }
}

internal enum State
{
    On,
    Off
}

internal enum Trigger
{
    Toggle
}