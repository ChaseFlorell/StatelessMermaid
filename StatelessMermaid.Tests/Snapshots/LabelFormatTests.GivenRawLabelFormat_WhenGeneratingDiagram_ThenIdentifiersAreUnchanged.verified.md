```mermaid
stateDiagram-v2
	direction TB
	[*] --> WaitingForInput
	WaitingForInput --> ProcessingComplete: ProcessData
	ProcessingComplete --> [*]
	note right of WaitingForInput
		entry / OnEnterWaitingForInput
	end note
```
