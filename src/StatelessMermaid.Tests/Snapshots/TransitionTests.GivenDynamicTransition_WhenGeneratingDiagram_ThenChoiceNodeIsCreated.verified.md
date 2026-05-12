```mermaid
stateDiagram-v2
	direction TB
	[*] --> A
	state A_Route_choice <<choice>>
	A --> A_Route_choice: Route
	A_Route_choice --> B: condition true
	A_Route_choice --> C: condition false
	B --> [*]
	C --> [*]
```
