using System;

namespace StatelessMermaid.Internal;

internal class DisposeAction : IDisposable
{
    public DisposeAction(Action disposeAction) => _disposeAction = disposeAction;

    public void Dispose() => _disposeAction();

    private readonly Action _disposeAction;
}