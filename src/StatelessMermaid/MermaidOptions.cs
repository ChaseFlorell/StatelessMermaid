using System;
using StatelessMermaid.Internal;

namespace StatelessMermaid;

/// <summary>
/// Configures how a Mermaid state diagram is rendered by <c>stateMachine.ToMermaid()</c>.
/// </summary>
public sealed record MermaidOptions
{
    /// <summary>
    /// Replaces the process-wide default <see cref="MermaidOptions"/> instance returned by <see cref="Default"/>.
    /// Call this once at application startup to avoid passing <see cref="MermaidOptions"/> explicitly on every call to
    /// <c>stateMachine.ToMermaid()</c>.
    /// </summary>
    /// <param name="options">The <see cref="MermaidOptions"/> instance to use as the new default.</param>
    /// <remarks>Resets defaults on Dispose()</remarks>
    public static IDisposable ConfigureDefaults(MermaidOptions options)
    {
        Default = options;
        return new DisposeAction(() => Default = new MermaidOptions());
    }

    /// <summary>A ready-made instance using all default values.</summary>
    public static MermaidOptions Default { get; private set; } = new();

    /// <summary>
    /// When <see langword="true"/> (default), wraps the output in a fenced <c>```mermaid</c> code block
    /// suitable for embedding directly in Markdown documents.
    /// Set to <see langword="false"/> when passing the diagram to a renderer that expects raw Mermaid syntax.
    /// </summary>
    public bool IncludeMarkdownBlocks { get; init; } = true;

    /// <summary>
    /// Controls the layout direction of the diagram. Defaults to <see cref="DiagramDirection.TopToBottom"/>.
    /// Also determines note placement: <see cref="DiagramDirection.RightToLeft"/> places notes on the left;
    /// all other directions place notes on the right.
    /// </summary>
    public DiagramDirection Direction { get; init; } = DiagramDirection.TopToBottom;

    /// <summary>
    /// The Mermaid <c>stateDiagram</c> syntax version to emit. Defaults to <see cref="DiagramVersion.V2"/>,
    /// which is required for composite states, notes, and choice nodes.
    /// </summary>
    public DiagramVersion Version { get; init; } = DiagramVersion.V2;

    /// <summary>
    /// Optional title rendered above the diagram via Mermaid's front-matter block.
    /// When <see langword="null"/> or empty, no title block is emitted.
    /// </summary>
    public string? Title { get; init; }
}