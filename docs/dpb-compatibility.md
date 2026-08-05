# DPB Compatibility

## Purpose

Record the external contract that cannot be inferred from this repository alone.

## Audience

Developers building or releasing the plugin.

## Update when

Update when the DPB version, assembly API, UI tree, pathfinder or deployment
layout changes.

## Depends on

[`AllflameBot.csproj`](../AllflameBot.csproj), [`3rdparty.json`](../3rdparty.json)
and the actual DPB binaries used for a build.

## Current constraints

- Target framework: .NET Framework 4.8.
- Platform: x64.
- Language: C# 7.3.
- External references: `DreamPoeBot.exe` and `Newtonsoft.Json.dll`.
- Runtime integrations include `LokiPoe`, `ExilePather`, `Element`, inventory/UI
  controls, process hook and `LokiCoroutine`.

## Compatibility checklist

- Compile against the same DPB binary family used for deployment.
- Confirm every accessed API exists and has the expected signature.
- Re-check stable UI child positions and labels after client/DPB updates.
- Re-check `TerrainData.Cache`, rows/columns, nav-cell size and walkability
  semantics.
- Re-check input APIs and ensure all held keys/buttons are released.
- Record the exact DPB build in release notes when a compatibility fix is made.

## Unknowns

The DPB binary is external to this repository. Terrain completeness, coordinate
origin and UI exposure must be verified at runtime; they are not guaranteed by
the plugin source alone.
