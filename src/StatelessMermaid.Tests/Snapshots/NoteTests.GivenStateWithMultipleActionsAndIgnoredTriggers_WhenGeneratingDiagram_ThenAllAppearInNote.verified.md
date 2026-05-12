```mermaid
stateDiagram-v2
	direction TB
	[*] --> Alpha
	Alpha --> Beta: Next
	Beta --> Gamma: Next
	Gamma --> [*]
	note right of Alpha
		entry / OnEnterAlpha
		exit / OnExitAlpha
		ignore: Ping
	end note
	note right of Beta
		entry / OnEnterBeta
	end note
```
