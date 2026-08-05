# Dependency Map

## Purpose

Show the important compile-time and runtime dependency directions.

## Audience

Agents reviewing coupling, refactors and external API changes.

## Update when

Update when a new external API, integration, service or module dependency is
introduced.

## Depends on

[`architecture/overview.md`](overview.md),
[`docs/dpb-compatibility.md`](../docs/dpb-compatibility.md) and
[`3rdparty.json`](../3rdparty.json).

## External boundaries

| Boundary | Used for | Current location |
| --- | --- | --- |
| DPB bot contracts | `IBot`, `IPlayerMover`, task manager and lifecycle | Host and mover classes |
| `LokiPoe` | Game state, objects, inventory, UI, input and process hook | Context, tasks and adapters |
| `ExilePather` / `AreaGraphs` | Path and topology queries | Mover and planners |
| `Element` tree | Stable DPB UI discovery and interaction | UI adapter region |
| GDI screenshot APIs | Visual fallback matching | UI fallback region |
| `poe.ninja` | Price feed | `AllflamePrices` |
| Discord webhook | Session dashboard and alerts | `AllflameNotifications` |
| Local JSON/files | Settings, prices, mapping and backups | Infrastructure |

## Target direction

`Host -> Runtime -> Features -> Context/State -> Adapters/Infrastructure` is
the preferred dependency direction. Adapters may depend on DPB; domain state
must not depend on adapters. The current implementation violates this in places
because tasks and state helpers are co-located in `AllflameBot.cs`; document the
violation before refactoring it.
