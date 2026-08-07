# Architecture Overview

## Purpose

Describe the current architecture and the boundaries an agent must preserve.

## Audience

Developers, reviewers and AI agents changing runtime behavior.

## Source of truth

The source remains authoritative. This document summarizes the behavior visible
in [`AllflameBot.cs`](../AllflameBot.cs), the auxiliary `.cs` files and the
project manifest.

## Update when

Update when task ownership, state ownership, external dependencies, build
constraints or fallback policy changes.

## Depends on

[`architecture/runtime-flow.md`](runtime-flow.md),
[`architecture/state-model.md`](state-model.md),
[`architecture/dependency-map.md`](dependency-map.md) and the module contexts in
[`docs/modules/`](../docs/modules/).

## Current shape

The plugin is one .NET Framework 4.8 x64 assembly. `AllflameBot` implements the
DreamPoeBot `IBot` contract and registers an ordered `ObserverTask` pipeline.
`AllflameMover` implements the separate DPB player-mover contract but shares
settings, movement and logging conventions.

The code has logical layers, not physical project boundaries:

1. Host lifecycle and task orchestration.
2. Context observation and global run state.
3. Chart, Voyage, loot, stash, extraction and bottle workflows.
4. Navigation and map planners.
5. DPB UI/input adapters and visual fallbacks.
6. Configuration, persistence, pricing, logging and notifications.

Most of these layers currently live in the large `AllflameBot.cs` file. The
target documentation structure names the boundaries before any source split.

## Runtime ownership

- `AllflameContext` owns the current observation snapshot.
- `UpdateDeepwaterStateTask` refreshes context each cycle.
- `VoyageRunState`, `AllflameRunState` and `AllflameLootState` own domain
  progress and reservations.
- `AllflameMovementCoordinator` owns movement priority and leases.
- `AllflameProgressWatchdog` owns progress observations and recoverable retries.
- `AllflameStopController` owns terminal stop requests; existing stop paths are
  legacy behavior and must not be expanded casually.
- Feature tasks own actions, but they must verify results through a later
  observation or an explicit safe postcondition.

## Target boundary rules

- Direct DPB calls belong in host, DPB adapter, UI adapter and navigation
  integration code.
- State objects do not perform network, filesystem or UI work.
- Feature workflows consume context and services rather than reaching into DPB
  everywhere.
- Notifications observe session/domain results and never decide movement. Discord
  delivery is queued outside the main task cycle; transport failures cannot
  request movement, recovery or terminal stop.
- Fallbacks must be ordered, logged and bounded per action; they must not silently
  become a second competing state machine.
