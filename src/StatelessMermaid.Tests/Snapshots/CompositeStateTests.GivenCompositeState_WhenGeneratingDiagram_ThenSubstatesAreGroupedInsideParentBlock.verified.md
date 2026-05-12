```mermaid
stateDiagram-v2
	direction TB
	[*] --> Offline
	state Online {
		Idle --> Busy: Start
		Busy --> Idle: Finish
	}
	Offline --> Idle: Connect
	Online --> Offline: Disconnect
```
