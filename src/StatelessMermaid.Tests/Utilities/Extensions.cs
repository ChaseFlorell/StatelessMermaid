using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Primitives;
using VerifyTests;
using VerifyXunit;

namespace StatelessMermaid.Tests.Utilities;

internal static class Extensions
{
    extension(StringAssertions assertions)
    {
        public async Task VerifyAsync()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(assertions.Subject));
            var verify = Verifier.Verify(stream, "md");

            await verify;
        }

        public async Task VerifyAsync(Func<SettingsTask, Task> settings)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(assertions.Subject));
            var verify = Verifier.Verify(stream, "md");

            await settings(verify);
        }
    }
}