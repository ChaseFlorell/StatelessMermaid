#!/usr/bin/env bash
set -euo pipefail

dotnet pack StatelessMermaid/StatelessMermaid.csproj \
  --no-restore \
  --configuration Release \
  --output out \
  /p:Version="$VERSION"
