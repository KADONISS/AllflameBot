# Release Guide

## Purpose

Describe the supported manual release contract for the plugin DLL, deployment
data and public release metadata.

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

## Distribution contract

The deployable unit is the `AllflameBot.dll` assembly plus the matching `Data`
content required by the plugin. DPB host binaries, user settings, logs and
generated build output are not part of the source repository.

The source commit is the canonical versioned record. A public GitHub Release
adds a version tag and downloadable assets:

- The tag must use the `v<version>` format and resolve to the release commit.
- `AllflameBot.dll` is the minimum release asset and must match the source
  version and commit build.
- A DLL-only download is not a complete deployment when the matching `Data`
  content is not already installed. A self-contained package may be published
  as `AllflameBot-<version>.zip` with the DLL and `Data` together.

## Required metadata

- Project version and assembly version.
- Runtime `AllflameVersion` marker.
- `AllflameMover.Version` marker.
- Changelog entry.
- Matching DPB dependency/build identity.
- Commit SHA.
- Version tag and public release URL, when a GitHub Release is published.
- SHA256 for every published binary asset.

Do not claim a release artifact exists until the DLL has been built and its
metadata/output has been checked. Do not claim a self-contained package exists
unless the matching `Data` content is present in that package or is explicitly
available from the same source tag.
