```mermaid
stateDiagram-v2
	direction TB
	[*] --> Leaf
	state Root {
		state Mid {
		}
	}
	Root --> Outside: Disconnect
	Leaf --> Outside: Finish
	Outside --> [*]
```
