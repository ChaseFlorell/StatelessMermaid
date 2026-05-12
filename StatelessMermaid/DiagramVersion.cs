namespace StatelessMermaid;

/// <summary>
/// Specifies which version of the Mermaid <c>stateDiagram</c> syntax to emit.
/// </summary>
public enum DiagramVersion
{
    /// <summary>Emits <c>stateDiagram</c> (original syntax, limited feature support).</summary>
    V1,

    /// <summary>Emits <c>stateDiagram-v2</c> (recommended; supports composite states, notes, and choice nodes).</summary>
    V2
}