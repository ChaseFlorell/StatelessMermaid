using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace StatelessMermaid.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() =>
        Verifier.DerivePathInfo((_, projectDirectory, _, _) =>
            new PathInfo(Path.Combine(projectDirectory, "Snapshots")));
}

[CollectionDefinition("sequential", DisableParallelization = true)]
public class SequentialCollection { }
