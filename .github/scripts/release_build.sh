#!/usr/bin/env bash
set -euo pipefail

dotnet build --no-restore --configuration Release /p:Version="$VERSION"
