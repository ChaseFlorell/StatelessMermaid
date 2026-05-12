## Installation

Install from NuGet:
[![NuGet](https://img.shields.io/badge/nuget-v${VERSION}-blue)](https://www.nuget.org/packages/StatelessMermaid/${VERSION})

Install via .NET CLI:

```bash
dotnet add package StatelessMermaid --version ${VERSION}
```

## Provenance

| Commit | Build |
|--------|-------|
| [`${SHA}`](https://github.com/${REPOSITORY}/commit/${SHA}) | [${RUN_ID}](https://github.com/${REPOSITORY}/actions/runs/${RUN_ID}) |

Verify package attestation:

```bash
gh attestation verify <path-to-nupkg> --repo ${REPOSITORY}
```
