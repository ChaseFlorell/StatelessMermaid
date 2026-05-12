using System.Threading.Tasks;
using FluentAssertions;
using StatelessMermaid.Tests.Fixtures;
using StatelessMermaid.Tests.Utilities;
using Xunit;

namespace StatelessMermaid.Tests;

public class DiagramOptionsTests
{
    [Fact]
    public async Task GivenDefaultOptions_WhenGeneratingDiagram_ThenOutputIsWrappedInMarkdownBlock()
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenMarkdownBlocksDisabled_WhenGeneratingDiagram_ThenOutputIsRawMermaidSyntax()
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { IncludeMarkdownBlocks = false });
        
        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenTitleIsSet_WhenGeneratingDiagram_ThenFrontMatterTitleBlockIsIncluded()
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Title = "Toggle Machine" });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenEmptyTitle_WhenGeneratingDiagram_ThenNoFrontMatterBlockIsEmitted()
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Title = "" });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenWhitespaceTitle_WhenGeneratingDiagram_ThenNoFrontMatterBlockIsEmitted()
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Title = "   " });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Theory]
    [InlineData(DiagramDirection.TopToBottom)]
    [InlineData(DiagramDirection.LeftToRight)]
    [InlineData(DiagramDirection.RightToLeft)]
    [InlineData(DiagramDirection.BottomToTop)]
    public async Task GivenDirection_WhenGeneratingDiagram_ThenDirectionKeywordMatchesOption(DiagramDirection direction)
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Direction = direction });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync(settings => settings.UseParameters(direction));
    }

    [Theory]
    [InlineData(DiagramVersion.V1)]
    [InlineData(DiagramVersion.V2)]
    public async Task GivenVersion_WhenGeneratingDiagram_ThenStateDiagramKeywordMatchesVersion(DiagramVersion version)
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Version = version });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync(settings => settings.UseParameters(version));
    }

    [Fact]
    public async Task GivenUnknownDirection_WhenGeneratingDiagram_ThenDefaultDirectionIsUsed()
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Direction = (DiagramDirection)99 });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public async Task GivenUnknownVersion_WhenGeneratingDiagram_ThenDefaultVersionIsUsed()
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();

        // Act
        var act = () => machine.ToMermaid(new MermaidOptions { Version = (DiagramVersion)99 });

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }
}