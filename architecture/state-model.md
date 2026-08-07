# State Model

## Purpose

Define the current state stores, their ownership and reset boundaries.

## Audience

Agents changing planners, extraction, loot, recovery or task transitions.

## Update when

Update when a state property is added, moved, renamed or changes lifecycle.

## Depends on

[`architecture/runtime-flow.md`](runtime-flow.md),
[`knowledge/map-model.md`](../knowledge/map-model.md) and the relevant module
context.

| State store | Owns | Reset boundary | Main consumers |
| --- | --- | --- | --- |
| `AllflameContext` | Phase, area, player, objects, UI and inventory snapshot | Bot reset or observation invalidation | All tasks and planners |
| `VoyageRunState` | Voyage spawn, capsules, lanterns, breadcrumbs, sectors, extraction | Voyage entry/return | Voyage, capsule, lantern and extraction tasks |
| `AllflameRunState` | Chart request, planning, chart spawn, exploration, boat staging and pre-Voyage non-Chart stash completion | Chart entry/return | Chart, stash, planning and chart exploration tasks |
| `AllflameLootState` | Chest session, reserved drops, bottle chest grace, item debt and committed Chart chest loot | Chest loot completion or area change | Chest, loot, recovery and return tasks |
| `AllflameProgressWatchdog` | Active task, target, retry and progress timestamps | Task completion or runtime reset | Base task and recovery logic |
| `AllflameMovementCoordinator` | Movement owner, priority, lease and navigation revision | Area change or explicit release | Tasks and `AllflameMover` |
| `AllflameStopController` | Stop request, reason and detail | Bot start | Bot lifecycle and notifications |
| `AllflameSession` | Session ledger, map counts, loot and profit | Session start/stop | Notifications and dashboard |

## Mutation rules

- Context observation should be the only source of current world facts.
- Run state may remember intent, progress, reservations and proven facts, but it
  must not pretend a stale observation is current.
- A task may mutate its owned progress state; shared state mutations need a
  state signature or structured log when they affect task arbitration.
- Area hash changes invalidate coordinates, planners, cached objects and active
  movement leases.
- A committed Chart chest loot reservation is valid only for its original area
  and Chart phase; a confirmed `0/x` transition may cancel unopened chest work
  but must not clear that reservation before loot completion.
- Extraction and capsule reservations have higher movement priority than normal
  exploration.
