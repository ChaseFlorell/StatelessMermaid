using System.Threading.Tasks;
using FluentAssertions;
using StatelessMermaid.Tests.Fixtures;
using StatelessMermaid.Tests.Utilities;
using Xunit;

namespace StatelessMermaid.Tests;

public class FullMachineTests
{
    [Fact]
    public async Task GivenFullStateMachine_WhenGeneratingDiagramWithDefaultOptions_ThenDiagramIsComplete()
    {
        // Arrange
        var machine = new FullStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenFullStateMachine_WhenGeneratingDiagramWithTitle_ThenTitleAppearsInFrontMatter()
    {
        // Arrange
        var machine = new FullStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Title = "Device State Machine" });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenFullStateMachine_WhenGeneratingDiagramLeftToRight_ThenDirectionIsLR()
    {
        // Arrange
        var machine = new FullStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Direction = DiagramDirection.LeftToRight });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenFullStateMachine_WhenGeneratingDiagramWithoutMarkdownBlocks_ThenOutputIsRawMermaidSyntax()
    {
        // Arrange
        var machine = new FullStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(MermaidOptions.Default with { IncludeMarkdownBlocks = false });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }
}
