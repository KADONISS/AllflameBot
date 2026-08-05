# Release Guide

## Purpose

Describe the supported manual release contract for the plugin DLL.

## Audience

Developers preparing a build for DPB deployment.

## Update when

Update when build commands, output paths, manifest fields or release checks
change.

## Depends on

[`workflows/release.md`](../workflows/release.md),
[`rules/release.md`](../rules/release.md),
[`docs/dpb-compatibility.md`](dpb-compatibility.md) and
[`AllflameBot.csproj`](../AllflameBot.csproj).

## Current release unit

The release unit is the `AllflameBot.dll` assembly plus the `Data` content
required by the plugin. DPB host binaries and user settings are not part of the
repository release.

## Required metadata

- Project version and assembly version.
- Runtime `AllflameVersion` marker.
- `AllflameMover.Version` marker.
- Changelog entry.
- Matching DPB dependency/build identity.
- Commit SHA.

Do not claim a release artifact exists until the DLL has been built and its
metadata/output has been checked.
