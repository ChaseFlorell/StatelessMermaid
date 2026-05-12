```mermaid
stateDiagram-v2
	direction RL
	[*] --> Alpha
	Alpha --> Beta: Next
	Beta --> [*]
	note left of Alpha
		entry / OnEnterAlpha
	end note
```
