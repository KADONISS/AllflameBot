# ADR 0001: Single DPB Plugin Assembly

- Status: Accepted (documented current contract)
- Date: 2026-08-06
- Depends on: `AllflameBot.csproj`, `3rdparty.json`, `docs/dpb-compatibility.md`

## Context

DreamPoeBot loads the plugin through its third-party plugin contract. The
project currently builds an assembly named `AllflameBot` and ships a `Data`
content file beside it.

## Decision

Keep one `AllflameBot.dll` deployment unit unless the DPB loading contract is
explicitly changed and validated.

## Consequences

- Logical source folders may be introduced without splitting assemblies.
- Project and `3rdparty.json` changes must be reviewed together.
- External DPB references remain deployment prerequisites.

## Alternatives rejected

Splitting runtime features into multiple plugin assemblies would add a host
loading contract that is not present in the current repository.
