```mermaid
stateDiagram-v2
	direction TB
	[*] --> A
	A --> B: Go
	B --> [*]
	note right of A
		ignore: Ping
	end note
```
