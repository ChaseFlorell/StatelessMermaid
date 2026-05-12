```mermaid
stateDiagram-v2
	direction TB
	[*] --> A
	state A_Go_choice <<choice>>
	A --> A_Go_choice: Go
	A_Go_choice --> B
	A_Go_choice --> C
	B --> [*]
	C --> [*]
```
