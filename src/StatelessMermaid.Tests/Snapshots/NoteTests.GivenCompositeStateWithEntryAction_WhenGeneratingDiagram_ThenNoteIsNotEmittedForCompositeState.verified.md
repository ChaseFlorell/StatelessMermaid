```mermaid
stateDiagram-v2
	direction TB
	[*] --> Alpha
	state Alpha {
	}
	Beta --> Gamma: Next
	Gamma --> [*]
	note right of Beta
		entry / OnEnterBeta
	end note
```
