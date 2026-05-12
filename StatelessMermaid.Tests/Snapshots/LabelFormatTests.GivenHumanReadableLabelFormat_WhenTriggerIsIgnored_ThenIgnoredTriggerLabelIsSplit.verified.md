```mermaid
stateDiagram-v2
	direction TB
	[*] --> WaitingForInput
	WaitingForInput: Waiting For Input
	ProcessingComplete: Processing Complete
	WaitingForInput --> ProcessingComplete: Go To Idle
	ProcessingComplete --> [*]
	note right of WaitingForInput
		ignore: Process Data
	end note
```
