```mermaid
stateDiagram-v2
	direction TB
	[*] --> WaitingForInput
	WaitingForInput: Waiting For Input
	ProcessingComplete: Processing Complete
	WaitingForInput --> ProcessingComplete: Process Data
	ProcessingComplete --> [*]
```
