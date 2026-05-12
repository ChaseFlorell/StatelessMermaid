```mermaid
stateDiagram-v2
	direction TB
	[*] --> On
	On: Powered On
	Standby: Stand-By Mode
	On --> Off: Switch
	On --> Standby: Sleep
	Off --> On: Switch
	Standby --> On: Wake
```
