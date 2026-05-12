using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Stateless;
using StatelessMermaid.Tests.Utilities;
using Xunit;

namespace StatelessMermaid.Tests;

public class TriggerTypeDisplayTests
{
    [Fact]
    public async Task GivenTriggerTypeDisplayNone_WhenTriggerIsParameterized_ThenTypeAnnotationIsOmitted()
    {
        var machine = new StateMachine<State, Trigger>(State.A);
        var paramTrigger = machine.SetTriggerParameters<int>(Trigger.Go);
        machine.Configure(State.A).PermitIf(paramTrigger, State.B, n => n > 0);
        machine.Configure(State.B);

        var act = () => machine.ToMermaid(new MermaidOptions { TriggerTypeDisplay = TriggerTypeDisplayMode.None });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    [Fact]
    public async Task GivenTriggerTypeDisplayFriendlyAlias_WhenTriggerIsParameterized_ThenCSharpAliasAppears()
    {
        var machine = new StateMachine<State, Trigger>(State.A);
        var paramTrigger = machine.SetTriggerParameters<int>(Trigger.Go);
        machine.Configure(State.A).PermitIf(paramTrigger, State.B, n => n > 0);
        machine.Configure(State.B);

        var act = () => machine.ToMermaid(new MermaidOptions { TriggerTypeDisplay = TriggerTypeDisplayMode.FriendlyAlias });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    [Fact]
    public async Task GivenTriggerTypeDisplayClrName_WhenTriggerIsParameterized_ThenClrTypeNameAppears()
    {
        var machine = new StateMachine<State, Trigger>(State.A);
        var paramTrigger = machine.SetTriggerParameters<int>(Trigger.Go);
        machine.Configure(State.A).PermitIf(paramTrigger, State.B, n => n > 0);
        machine.Configure(State.B);

        var act = () => machine.ToMermaid(new MermaidOptions { TriggerTypeDisplay = TriggerTypeDisplayMode.ClrName });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    [Fact]
    public async Task GivenTriggerTypeDisplayClrName_WhenTriggerHasMultipleParameters_ThenAllClrNamesAppear()
    {
        var machine = new StateMachine<State, Trigger>(State.A);
        var paramTrigger = machine.SetTriggerParameters<int, string>(Trigger.Go);
        machine.Configure(State.A).PermitIf(paramTrigger, State.B, (n, _) => n > 0);
        machine.Configure(State.B);

        var act = () => machine.ToMermaid(new MermaidOptions { TriggerTypeDisplay = TriggerTypeDisplayMode.ClrName });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    [Fact]
    public async Task GivenTriggerTypeDisplayNone_WhenIgnoredTriggerIsParameterized_ThenTypeAnnotationIsOmitted()
    {
        var machine = new StateMachine<State, Trigger>(State.A);
        machine.SetTriggerParameters<int>(Trigger.Ping);
        machine.Configure(State.A)
            .Ignore(Trigger.Ping)
            .Permit(Trigger.Go, State.B);
        machine.Configure(State.B);

        var act = () => machine.ToMermaid(new MermaidOptions { TriggerTypeDisplay = TriggerTypeDisplayMode.None });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    [Fact]
    public async Task GivenTriggerTypeDisplayClrName_WhenTriggerHasGenericParameterType_ThenClrNameIsUnformatted()
    {
        var machine = new StateMachine<State, Trigger>(State.A);
        var paramTrigger = machine.SetTriggerParameters<List<int>>(Trigger.Go);
        machine.Configure(State.A).PermitIf(paramTrigger, State.B, _ => true);
        machine.Configure(State.B);

        var act = () => machine.ToMermaid(new MermaidOptions { TriggerTypeDisplay = TriggerTypeDisplayMode.ClrName });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    [Fact]
    public async Task GivenTriggerTypeDisplayNoneWithHumanReadableLabels_WhenGeneratingDiagram_ThenBothOptionsApply()
    {
        var machine = new StateMachine<State, Trigger>(State.A);
        var paramTrigger = machine.SetTriggerParameters<int>(Trigger.Go);
        machine.Configure(State.A).PermitIf(paramTrigger, State.B, n => n > 0);
        machine.Configure(State.B);

        var act = () => machine.ToMermaid(new MermaidOptions
        {
            TriggerTypeDisplay = TriggerTypeDisplayMode.None,
            LabelFormat = LabelFormat.HumanReadable
        });

        await act.Should().NotThrow().Which.Should().VerifyAsync();
    }

    private enum State { A, B }
    private enum Trigger { Go, Ping }
}
