# DPB API Boundary

## Purpose

Record the external DreamPoeBot surface used by the plugin.

## Audience

Agents changing host integration, pathfinding, UI or input code.

## Update when

Update when the supported DPB version or accessed API surface changes.

## Depends on

[`docs/dpb-compatibility.md`](../../docs/dpb-compatibility.md) and the external
assemblies referenced by [`AllflameBot.csproj`](../../AllflameBot.csproj).

## Rules

- Verify API names and signatures against the actual referenced assembly.
- Do not infer terrain origin, tile size or UI tree indices from screenshots.
- Keep direct DPB calls close to the adapter that owns their fallback behavior.
- Log the selected adapter mode when a feature has multiple input paths.
- Treat unavailable DPB APIs as a compatibility issue, not as proof that the
  game state does not exist.
