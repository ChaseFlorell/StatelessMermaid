# StatelessMermaid

[![NuGet](https://img.shields.io/nuget/v/StatelessMermaid)](https://www.nuget.org/packages/StatelessMermaid/)
[![codecov](https://codecov.io/gh/ChaseFlorell/StatelessMermaid/branch/main/graph/badge.svg)](https://codecov.io/gh/ChaseFlorell/StatelessMermaid)
[![Build](https://github.com/ChaseFlorell/StatelessMermaid/actions/workflows/main-build.yml/badge.svg)](https://github.com/ChaseFlorell/StatelessMermaid/actions/workflows/main-build.yml)

Generates [Mermaid](https://mermaid.js.org/) state diagrams from [Stateless](https://github.com/dotnet-state-machine/stateless) state machines via a single extension method.

## Installation

```bash
dotnet add package StatelessMermaid
```

## Usage

Call `ToMermaid()` on any `StateMachine<TState, TTrigger>`:

```csharp
var machine = new StateMachine<State, Trigger>(State.Offline);
// ... configure machine ...

string diagram = machine.ToMermaid();
```

The output is a fenced ` ```mermaid ` code block ready to embed in a Markdown document, GitHub README, or any Mermaid-compatible renderer.

## Options

Pass a `MermaidOptions` instance to control rendering:

```csharp
string diagram = machine.ToMermaid(new MermaidOptions
{
    Title = "Device Lifecycle",
    Direction = DiagramDirection.LeftToRight,
    Version = DiagramVersion.V2,
    IncludeMarkdownBlocks = true,
});
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `null` | Adds a Mermaid front-matter title above the diagram. |
| `Direction` | `DiagramDirection` | `TopToBottom` | Layout direction: `TopToBottom`, `LeftToRight`, `RightToLeft`, `BottomToTop`. |
| `Version` | `DiagramVersion` | `V2` | `V2` (`stateDiagram-v2`, recommended) or `V1` (`stateDiagram`). Composite states, notes, and choice nodes require V2. |
| `IncludeMarkdownBlocks` | `bool` | `true` | Wraps output in a fenced ` ```mermaid ` block. Set to `false` when passing to a renderer that expects raw Mermaid syntax. |

## Process-wide defaults

To avoid passing options on every call, configure a default once at startup:

```csharp
MermaidOptions.ConfigureDefaults(new MermaidOptions
{
    Direction = DiagramDirection.LeftToRight,
});
```

`ConfigureDefaults` returns an `IDisposable` that resets to the original defaults on dispose, which is useful in tests.

## Provenance

Each release is published via an attested GitHub Actions build. To verify a package:

```bash
gh attestation verify <path-to-nupkg> --repo ChaseFlorell/StatelessMermaid
```

Full release history and attached `.nupkg` files are available on the [GitHub Releases](https://github.com/ChaseFlorell/StatelessMermaid/releases) page.

## License

[MIT](https://github.com/ChaseFlorell/StatelessMermaid/blob/main/LICENSE)
