namespace StatelessMermaid;

/// <summary>
/// Controls the layout direction of the generated Mermaid state diagram.
/// </summary>
public enum DiagramDirection
{
    /// <summary>States flow from top to bottom (default).</summary>
    TopToBottom,

    /// <summary>States flow from left to right.</summary>
    LeftToRight,

    /// <summary>States flow from right to left. Notes are placed to the left of each state.</summary>
    RightToLeft,

    /// <summary>States flow from bottom to top.</summary>
    BottomToTop
}