using System.ComponentModel;
using Stateless;
using Stateless.Reflection;

namespace StatelessMermaid.Tests.Fixtures;

internal class FullStateMachineFixture : StateMachine<FullStateMachineFixture.DeviceState, FullStateMachineFixture.DeviceTrigger>
{
    public FullStateMachineFixture() : base(DeviceState.Offline)
    {
        var diagnoseTrigger = SetTriggerParameters<DiagnosticReport>(DeviceTrigger.Diagnose);

        Configure(DeviceState.Offline)
            .OnEntry(OnEnterOffline)
            .Permit(DeviceTrigger.Connect, DeviceState.Idle);

        Configure(DeviceState.Online)
            .OnEntry(OnEnterOnline)
            .OnExit(OnExitOnline)
            .InternalTransition(DeviceTrigger.Ping, _ => HandlePing())
            .Ignore(DeviceTrigger.Connect)
            .Permit(DeviceTrigger.Disconnect, DeviceState.Offline)
            .PermitDynamic(
                DeviceTrigger.Fault,
                () => DeviceState.Warning,
                "Check fault level",
                new DynamicStateInfos
                {
                    { DeviceState.CriticalError, "faultLevel > 5" },
                    { DeviceState.Warning, "faultLevel <= 5" }
                });

        Configure(DeviceState.Idle)
            .SubstateOf(DeviceState.Online)
            .OnEntry(OnEnterIdle)
            .Permit(DeviceTrigger.StartWork, DeviceState.Processing)
            .PermitIf(diagnoseTrigger, DeviceState.Warning, r => r.Severity < 5)
            .PermitIf(diagnoseTrigger, DeviceState.CriticalError, r => r.Severity >= 5 || r.RequiresShutdown);

        Configure(DeviceState.Processing)
            .SubstateOf(DeviceState.Online)
            .OnEntry(OnEnterProcessing)
            .OnExit(OnExitProcessing)
            .Ignore(DeviceTrigger.StartWork)
            .Permit(DeviceTrigger.PauseWork, DeviceState.Paused)
            .Permit(DeviceTrigger.CompleteWork, DeviceState.Idle);

        Configure(DeviceState.Paused)
            .SubstateOf(DeviceState.Online)
            .OnEntry(OnEnterPaused)
            .Permit(DeviceTrigger.ResumeWork, DeviceState.Processing)
            .Permit(DeviceTrigger.CompleteWork, DeviceState.Idle);

        Configure(DeviceState.Warning)
            .OnEntry(OnEnterWarning)
            .Permit(DeviceTrigger.Acknowledge, DeviceState.Idle)
            .Permit(DeviceTrigger.Reset, DeviceState.Offline);

        Configure(DeviceState.CriticalError)
            .OnEntry(OnEnterCriticalError)
            .Permit(DeviceTrigger.Reset, DeviceState.Offline)
            .Permit(DeviceTrigger.Decommission, DeviceState.Decommissioned);

        Configure(DeviceState.Decommissioned)
            .OnEntry(OnEnterDecommissioned);
    }

    internal enum DeviceState
    {
        Offline,
        Online,
        Warning,
        [Description("Critical Error")] CriticalError,
        Decommissioned,
        Idle,
        Processing,
        Paused
    }

    internal enum DeviceTrigger
    {
        Connect,
        Disconnect,
        StartWork,
        PauseWork,
        ResumeWork,
        CompleteWork,
        Ping,
        Fault,
        Diagnose,
        Acknowledge,
        Reset,
        Decommission
    }

    internal sealed record DiagnosticReport(int Severity, string Component, bool RequiresShutdown);

    private static void OnEnterOffline()
    {
    }

    private static void OnEnterOnline()
    {
    }

    private static void OnExitOnline()
    {
    }

    private static void HandlePing()
    {
    }

    private static void OnEnterIdle()
    {
    }

    private static void OnEnterProcessing()
    {
    }

    private static void OnExitProcessing()
    {
    }

    private static void OnEnterPaused()
    {
    }

    private static void OnEnterWarning()
    {
    }

    private static void OnEnterCriticalError()
    {
    }

    private static void OnEnterDecommissioned()
    {
    }
}