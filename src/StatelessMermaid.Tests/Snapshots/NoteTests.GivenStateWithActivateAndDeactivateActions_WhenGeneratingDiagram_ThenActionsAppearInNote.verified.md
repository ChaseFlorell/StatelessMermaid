```mermaid
stateDiagram-v2
	direction TB
	[*] --> Alpha
	Alpha --> Beta: Next
	Beta --> [*]
	note right of Beta
		activate / OnActivateBeta
		deactivate / OnDeactivateBeta
	end note
```
