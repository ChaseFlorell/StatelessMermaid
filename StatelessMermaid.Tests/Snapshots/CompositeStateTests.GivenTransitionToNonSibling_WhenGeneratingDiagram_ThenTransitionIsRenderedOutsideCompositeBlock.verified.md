```mermaid
stateDiagram-v2
	direction TB
	[*] --> Offline
	state Online {
		Idle --> Busy: Start
		Busy --> Idle: Finish
	}
	Offline --> Idle: Connect
	Idle --> Offline: Disconnect
	Busy --> Offline: Disconnect
```
