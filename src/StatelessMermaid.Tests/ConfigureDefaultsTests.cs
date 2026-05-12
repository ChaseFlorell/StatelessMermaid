using System.Threading.Tasks;
using FluentAssertions;
using StatelessMermaid.Tests.Fixtures;
using StatelessMermaid.Tests.Utilities;
using Xunit;

namespace StatelessMermaid.Tests;

[Collection("sequential")]
public class ConfigureDefaultsTests
{
    [Fact]
    public void GivenCustomOptions_WhenConfiguringDefaults_ThenDefaultPropertyReturnsCustomOptions()
    {
        // Arrange
        var options = new MermaidOptions { Title = "Custom" };

        // Act / Assert
        using var _ = MermaidOptions.ConfigureDefaults(options);
        MermaidOptions.Default.Should().Be(options);
    }

    [Fact]
    public void GivenCustomOptions_WhenConfiguringDefaultsAndDisposing_ThenDefaultIsRestored()
    {
        // Arrange
        var options = new MermaidOptions { Title = "Custom" };

        // Act
        using (MermaidOptions.ConfigureDefaults(options))
        {
        }

        // Assert
        MermaidOptions.Default.Should().Be(new MermaidOptions());
    }

    [Fact]
    public async Task GivenCustomDefault_WhenGeneratingDiagramWithNoOptions_ThenCustomDefaultIsApplied()
    {
        // Arrange
        var machine = new DiagramOptionsStateMachineFixture();
        using var _ = MermaidOptions.ConfigureDefaults(new MermaidOptions { Title = "From Default", Direction = DiagramDirection.LeftToRight });

        // Act
        var act = () => machine.ToMermaid();

        // Assert
        await act.Should()
            .NotThrow()
            .Which.Should()
            .VerifyAsync();
    }

    [Fact]
    public void GivenCustomDefault_WhenExplicitOptionsArePassed_ThenExplicitOptionsAreUsed()
    {
        // Arrange
        var explicitOptions = new MermaidOptions { Title = "Explicit" };
        var machine = new DiagramOptionsStateMachineFixture();
        using var _ = MermaidOptions.ConfigureDefaults(new MermaidOptions { Title = "Default" });

        // Act
        var diagram = machine.ToMermaid(explicitOptions);

        // Assert
        diagram.Should().Contain("Explicit");
        diagram.Should().NotContain("Default");
    }
}