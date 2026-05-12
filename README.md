# StatelessMermaid

[![NuGet](https://img.shields.io/nuget/v/StatelessMermaid.svg)](https://www.nuget.org/packages/StatelessMermaid/)
[![codecov](https://codecov.io/gh/ChaseFlorell/StatelessMermaid/branch/main/graph/badge.svg)](https://codecov.io/gh/ChaseFlorell/StatelessMermaid)
[![Build](https://github.com/ChaseFlorell/StatelessMermaid/actions/workflows/main-build.yml/badge.svg)](https://github.com/ChaseFlorell/StatelessMermaid/actions/workflows/main-build.yml)

Generates [Mermaid](https://mermaid.js.org/) state diagrams from [Stateless](https://github.com/dotnet-state-machine/stateless) state machines via a single extension method.

## Installation

```
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

### Options

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

#### Process-wide defaults

To avoid passing options on every call, configure a default once at startup:

```csharp
MermaidOptions.ConfigureDefaults(new MermaidOptions
{
    Direction = DiagramDirection.LeftToRight,
});
```

`ConfigureDefaults` returns an `IDisposable` that resets to the original defaults on dispose, which is useful in tests.

## Example

### State machine configuration

```csharp
using System.ComponentModel;
using Stateless;
using StatelessMermaid;

var machine = new StateMachine<DeviceState, DeviceTrigger>(DeviceState.Offline);

var diagnoseTrigger = machine.SetTriggerParameters<DiagnosticReport>(DeviceTrigger.Diagnose);

machine.Configure(DeviceState.Offline)
    .OnEntry(OnEnterOffline)
    .Permit(DeviceTrigger.Connect, DeviceState.Idle);

machine.Configure(DeviceState.Online)
    .OnEntry(OnEnterOnline)
    .OnExit(OnExitOnline)
    .InternalTransition(DeviceTrigger.Ping, _ => HandlePing())
    .Ignore(DeviceTrigger.Connect)
    .Permit(DeviceTrigger.Disconnect, DeviceState.Offline)
    .PermitDynamic(DeviceTrigger.Fault, () => DeviceState.Warning, "Check fault level",
        new DynamicStateInfos
        {
            { DeviceState.CriticalError, "faultLevel > 5" },
            { DeviceState.Warning,       "faultLevel <= 5" }
        });

machine.Configure(DeviceState.Idle)
    .SubstateOf(DeviceState.Online)
    .OnEntry(OnEnterIdle)
    .Permit(DeviceTrigger.StartWork, DeviceState.Processing)
    .PermitIf(diagnoseTrigger, DeviceState.Warning,       r => r.Severity < 5)
    .PermitIf(diagnoseTrigger, DeviceState.CriticalError, r => r.Severity >= 5 || r.RequiresShutdown);

machine.Configure(DeviceState.Processing)
    .SubstateOf(DeviceState.Online)
    .OnEntry(OnEnterProcessing)
    .OnExit(OnExitProcessing)
    .Ignore(DeviceTrigger.StartWork)
    .Permit(DeviceTrigger.PauseWork,    DeviceState.Paused)
    .Permit(DeviceTrigger.CompleteWork, DeviceState.Idle);

machine.Configure(DeviceState.Paused)
    .SubstateOf(DeviceState.Online)
    .OnEntry(OnEnterPaused)
    .Permit(DeviceTrigger.ResumeWork,   DeviceState.Processing)
    .Permit(DeviceTrigger.CompleteWork, DeviceState.Idle);

machine.Configure(DeviceState.Warning)
    .OnEntry(OnEnterWarning)
    .Permit(DeviceTrigger.Acknowledge, DeviceState.Idle)
    .Permit(DeviceTrigger.Reset,       DeviceState.Offline);

machine.Configure(DeviceState.CriticalError)
    .OnEntry(OnEnterCriticalError)
    .Permit(DeviceTrigger.Reset,         DeviceState.Offline)
    .Permit(DeviceTrigger.Decommission,  DeviceState.Decommissioned);

machine.Configure(DeviceState.Decommissioned)
    .OnEntry(OnEnterDecommissioned);

string diagram = machine.ToMermaid();

enum DeviceState
{
    Offline, Online, Warning,
    [Description("Critical Error")] CriticalError,
    Decommissioned, Idle, Processing, Paused
}

enum DeviceTrigger
{
    Connect, Disconnect, StartWork, PauseWork,
    ResumeWork, CompleteWork, Ping, Fault,
    Diagnose, Acknowledge, Reset, Decommission
}

record DiagnosticReport(int Severity, string Component, bool RequiresShutdown);
```

### Generated diagram

````markdown
```mermaid
stateDiagram-v2
	direction TB
	[*] --> Offline
	CriticalError: Critical Error
	state Online {
		Idle --> Processing: StartWork
		Processing --> Paused: PauseWork
		Processing --> Idle: CompleteWork
		Paused --> Processing: ResumeWork
		Paused --> Idle: CompleteWork
	}
	Offline --> Idle: Connect
	Online --> Online: [internal] Ping
	Online --> Offline: Disconnect
	state Online_Fault_choice <<choice>>
	Online --> Online_Fault_choice: Fault
	Online_Fault_choice --> CriticalError: faultLevel > 5
	Online_Fault_choice --> Warning: faultLevel <= 5
	state Idle_Diagnose_choice <<choice>>
	Idle --> Idle_Diagnose_choice: Diagnose(DiagnosticReport)
	Idle_Diagnose_choice --> Warning
	Idle_Diagnose_choice --> CriticalError
	Warning --> Idle: Acknowledge
	Warning --> Offline: Reset
	CriticalError --> Offline: Reset
	CriticalError --> Decommissioned: Decommission
	Decommissioned --> [*]
	note right of Offline
		entry / OnEnterOffline
	end note
	note right of Idle
		entry / OnEnterIdle
	end note
	note right of Processing
		entry / OnEnterProcessing
		exit / OnExitProcessing
		ignore: StartWork
	end note
	note right of Paused
		entry / OnEnterPaused
	end note
	note right of Warning
		entry / OnEnterWarning
	end note
	note right of CriticalError
		entry / OnEnterCriticalError
	end note
	note right of Decommissioned
		entry / OnEnterDecommissioned
	end note
```
````

Which renders as:

```mermaid
stateDiagram-v2
	direction TB
	[*] --> Offline
	CriticalError: Critical Error
	state Online {
		Idle --> Processing: StartWork
		Processing --> Paused: PauseWork
		Processing --> Idle: CompleteWork
		Paused --> Processing: ResumeWork
		Paused --> Idle: CompleteWork
	}
	Offline --> Idle: Connect
	Online --> Online: [internal] Ping
	Online --> Offline: Disconnect
	state Online_Fault_choice <<choice>>
	Online --> Online_Fault_choice: Fault
	Online_Fault_choice --> CriticalError: faultLevel > 5
	Online_Fault_choice --> Warning: faultLevel <= 5
	state Idle_Diagnose_choice <<choice>>
	Idle --> Idle_Diagnose_choice: Diagnose(DiagnosticReport)
	Idle_Diagnose_choice --> Warning
	Idle_Diagnose_choice --> CriticalError
	Warning --> Idle: Acknowledge
	Warning --> Offline: Reset
	CriticalError --> Offline: Reset
	CriticalError --> Decommissioned: Decommission
	Decommissioned --> [*]
	note right of Offline
		entry / OnEnterOffline
	end note
	note right of Idle
		entry / OnEnterIdle
	end note
	note right of Processing
		entry / OnEnterProcessing
		exit / OnExitProcessing
		ignore: StartWork
	end note
	note right of Paused
		entry / OnEnterPaused
	end note
	note right of Warning
		entry / OnEnterWarning
	end note
	note right of CriticalError
		entry / OnEnterCriticalError
	end note
	note right of Decommissioned
		entry / OnEnterDecommissioned
	end note
```

## Features

- **Composite states** — `SubstateOf` hierarchies render as nested Mermaid state blocks.
- **Choice nodes** — `PermitDynamic` and multiple `PermitIf` on the same trigger produce `<<choice>>` nodes.
- **Entry / exit actions** — rendered as notes attached to each state.
- **Internal transitions** — labeled with an `[internal]` prefix.
- **Ignored triggers** — listed in the state's note block.
- **Terminal states** — states with no outbound transitions to other states get a `[*]` end marker.
- **Parameterized triggers** — type names appear on the transition label (e.g. `Diagnose(DiagnosticReport)`).
- **`[Description]` labels** — states and triggers decorated with `System.ComponentModel.DescriptionAttribute` use that text as their display label; PascalCase names without a description are split into words automatically.

## License

MIT
