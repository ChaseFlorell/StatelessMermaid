```mermaid
stateDiagram-v2
	direction TB
	[*] --> Alpha
	Alpha --> Beta: Next
	Beta --> [*]
	note right of Alpha
		exit / OnExitAlpha
	end note
```
