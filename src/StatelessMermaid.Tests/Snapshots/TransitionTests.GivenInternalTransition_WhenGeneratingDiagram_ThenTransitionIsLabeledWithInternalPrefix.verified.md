```mermaid
stateDiagram-v2
	direction TB
	[*] --> A
	A --> A: [internal] Ping
	A --> B: Go
	B --> A: Back
```
