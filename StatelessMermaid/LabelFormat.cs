namespace StatelessMermaid;

/// <summary>
/// Controls how identifier-based labels (state names, trigger names, action method names) are formatted.
/// </summary>
public enum LabelFormat
{
    /// <summary>Identifiers are used exactly as written. This is the default.</summary>
    Raw,

    /// <summary>
    /// PascalCase and camelCase identifiers are split into space-separated words using standard
    /// word-boundary detection, including acronym boundaries.
    /// For example: <c>OnEnterIdle</c> → <c>On Enter Idle</c>, <c>HTTPClient</c> → <c>HTTP Client</c>.
    /// A <c>[Description]</c> attribute on a state always takes precedence over this setting.
    /// </summary>
    HumanReadable,
}
