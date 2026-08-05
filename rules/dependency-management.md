# Dependency Management Rules

## Purpose

Keep external DPB and package assumptions explicit.

## Audience

Agents changing project references or external integrations.

## Update when

Update when a reference, API version or deployment dependency changes.

## Rules

- Update `.csproj`, `3rdparty.json`, compatibility docs and release notes together.
- Do not commit external DPB binaries.
- Validate assembly identity and target framework before building.
- Record whether a dependency is compile-time, runtime or optional.
- Avoid adding a package when the target environment already supplies the API.
