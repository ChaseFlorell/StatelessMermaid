```mermaid
stateDiagram-v2
	direction TB
	[*] --> ActiveSession
	ActiveSession: Active User Session
	IdleMode: System Idle
	ActiveSession --> IdleMode: Process Data
	IdleMode --> [*]
```
