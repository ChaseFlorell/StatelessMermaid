#!/usr/bin/env bash
set -euo pipefail

envsubst < ./.github/markdown/release_create-github-release.md > "$RUNNER_TEMP/release-notes.md"
