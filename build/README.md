# Build Area

## Purpose

Reserve a home for deterministic build helpers without changing the current
project contract prematurely.

## Audience

Developers maintaining local or CI builds.

## Update when

Update when a build script is added or the project target changes.

## Current status

The repository currently builds through `AllflameBot.csproj`; no checked-in build
script or CI pipeline exists. A future script must validate external DPB paths,
target x64/net48 and fail clearly when dependencies are missing.
