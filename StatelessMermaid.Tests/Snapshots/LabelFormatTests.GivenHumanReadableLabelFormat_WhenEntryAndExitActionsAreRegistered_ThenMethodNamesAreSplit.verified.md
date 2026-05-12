```mermaid
stateDiagram-v2
	direction TB
	[*] --> WaitingForInput
	WaitingForInput: Waiting For Input
	ProcessingComplete: Processing Complete
	WaitingForInput --> ProcessingComplete: Process Data
	ProcessingComplete --> [*]
	note right of WaitingForInput
		entry / On Enter Waiting For Input
		exit / On Exit Waiting For Input
	end note
	note right of ProcessingComplete
		activate / On Activate Processing Complete
		deactivate / On Deactivate Processing Complete
	end note
```
