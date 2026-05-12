```mermaid
stateDiagram-v2
	direction LR
	[*] --> Alpha
	Alpha --> Beta: Next
	Beta --> [*]
	note right of Alpha
		entry / OnEnterAlpha
	end note
```
