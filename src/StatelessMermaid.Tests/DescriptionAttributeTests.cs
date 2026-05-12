using System.ComponentModel;
using System.Threading.Tasks;
using FluentAssertions;
using Stateless;
using StatelessMermaid.Tests.Utilities;
using Xunit;

namespace StatelessMermaid.Tests;

public class DescriptionAttributeTests
{
    [Fact]
    public async Task GivenStateWithDescriptionAttribute_WhenGeneratingDiagram_ThenDescriptionIsUsedAsStateLabel()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.On);
        machine.Configure(State.On)
            .Permit(Trigger.Switch, State.Off)
            .Permit(Trigger.Sleep, State.Standby);
        machine.Configure(State.Off)
            .Permit(Trigger.Switch, State.On);
        machine.Configure(State.Standby)
            .Permit(Trigger.Wake, State.On);

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenStateWithoutDescriptionAttribute_WhenGeneratingDiagram_ThenEnumNameIsUsedAsStateLabel()
    {
        // Arrange
        var machine = new StateMachine<State, Trigger>(State.Off);
        machine.Configure(State.Off).Permit(Trigger.Switch, State.On);
        machine.Configure(State.On).Permit(Trigger.Switch, State.Off);

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
        [Description("Powered On")] On,
        Off,
        [Description("Stand-By Mode")] Standby
    }

    private enum Trigger { Switch, Sleep, Wake }
}
