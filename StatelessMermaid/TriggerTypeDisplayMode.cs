namespace StatelessMermaid;

/// <summary>
/// Controls how parameterized trigger type annotations are rendered on transition labels.
/// </summary>
public enum TriggerTypeDisplayMode
{
    /// <summary>Type annotations are omitted — trigger labels show only the trigger name.</summary>
    None,

    /// <summary>
    /// C# type aliases are used: <c>int</c>, <c>bool</c>, <c>string</c>, <c>List&lt;int&gt;</c>, etc.
    /// This is the default.
    /// </summary>
    FriendlyAlias,

    /// <summary>CLR type names are used: <c>Int32</c>, <c>Boolean</c>, <c>String</c>, <c>List`1</c>, etc.</summary>
    ClrName,
}
