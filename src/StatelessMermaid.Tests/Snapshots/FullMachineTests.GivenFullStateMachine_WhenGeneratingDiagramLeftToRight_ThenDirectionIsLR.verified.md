```mermaid
stateDiagram-v2
	direction LR
	[*] --> Offline
	CriticalError: Critical Error
	state Online {
		Idle --> Processing: StartWork
		Processing --> Paused: PauseWork
		Processing --> Idle: CompleteWork
		Paused --> Processing: ResumeWork
		Paused --> Idle: CompleteWork
	}
	Offline --> Idle: Connect
	Online --> Online: [internal] Ping
	Online --> Offline: Disconnect
	state Online_Fault_choice <<choice>>
	Online --> Online_Fault_choice: Fault
	Online_Fault_choice --> CriticalError: faultLevel > 5
	Online_Fault_choice --> Warning: faultLevel <= 5
	state Idle_Diagnose_choice <<choice>>
	Idle --> Idle_Diagnose_choice: Diagnose(DiagnosticReport)
	Idle_Diagnose_choice --> Warning
	Idle_Diagnose_choice --> CriticalError
	Warning --> Idle: Acknowledge
	Warning --> Offline: Reset
	CriticalError --> Offline: Reset
	CriticalError --> Decommissioned: Decommission
	Decommissioned --> [*]
	note right of Offline
		entry / OnEnterOffline
	end note
	note right of Idle
		entry / OnEnterIdle
	end note
	note right of Processing
		entry / OnEnterProcessing
		exit / OnExitProcessing
		ignore: StartWork
	end note
	note right of Paused
		entry / OnEnterPaused
	end note
	note right of Warning
		entry / OnEnterWarning
	end note
	note right of CriticalError
		entry / OnEnterCriticalError
	end note
	note right of Decommissioned
		entry / OnEnterDecommissioned
	end note
```
