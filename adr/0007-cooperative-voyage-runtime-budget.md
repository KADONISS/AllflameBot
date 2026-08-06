# ADR 0007: Cooperative Voyage Runtime Budget

## Status

Accepted

## Context

Voyage exploration can spend a long time inside synchronous terrain, UI and
pathfinder work. Frontier, detour, short-escape and generic watchdog timeouts
then accumulate independently, making a stalled cycle expensive to diagnose
and allowing a target to consume too much of the run.

## Decision

Keep normal Voyage exploration cooperative across task cycles. Recovery refresh
requests are coalesced and processed by the next state-observation task unless
an immediate UI postcondition is required. Terrain walkability is rebuilt only
after two stable metadata samples and a changed area/terrain signature. Voyage
path queries are cached by area hash, navigation revision, terrain signature
and endpoints. Normal exploration uses an explicit progress watchdog and a
bounded per-target recovery budget; exhaustion rotates the strategy and does
not create a new terminal stop path.

## Consequences

- A slow recovery no longer forces a full UI/object scan in the movement cycle
  that requested it.
- Stable terrain and repeated path checks consume less CPU time during Voyage.
- `VOYAGE_CYCLE_METRICS`, `VOYAGE_RECOVERY_BUDGET` and related markers expose
  target distance, query counts, refresh cost and recovery use.
- A fresh state can be observed one task cycle later for recoveries that do not
  require an immediate postcondition; UI actions that require confirmation keep
  their synchronous follow-up read.

## Alternatives considered

- Increasing every timeout: rejected because it hides the cause and increases
  time spent on an unreachable target.
- Adding an automatic bot stop: rejected because these faults are recoverable
  and the project recovery policy prefers strategy rotation.
- Replacing the planner with a single terrain algorithm: rejected because DPB
  terrain completeness and coordinate semantics remain runtime-dependent.

## Evidence

- `AllflameBot.cs`: `VoyageRunnerMetrics`, `AreaMapSnapshotService` and
  `ExploreVoyageTask`.
- Runtime timing markers: `CONTEXT_REFRESH_TIMING`, `TASK_CYCLE_SLOW` and
  `VOYAGE_CYCLE_METRICS`.
