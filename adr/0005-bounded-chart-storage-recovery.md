# ADR 0005: Bounded Chart Storage Recovery

- Status: Accepted (documented current contract)
- Date: 2026-08-06
- Depends on: `adr/0002-ordered-observer-task-pipeline.md`,
  `adr/0003-layered-ui-fallbacks.md`

## Context

`OpenChartStorageTask` used to resolve the Chart Storage Element tree before
the Chart Storage window had been opened. On the ordinary boat scene this
could leave the ordered task cycle waiting inside DPB UI reads, preventing the
runner from reaching the storage interaction or its fallback source.

## Decision

Treat Chart Storage as a bounded cooperative transaction: prepare blocking UI,
move to the object, issue one interaction, wait for a fresh visible-UI
postcondition, then resolve the page layer. Bound open and resolve recovery to
three attempts, refresh/reacquire the UI after a timeout, and continue through
Voyage Stash when Chart Storage remains unavailable.

## Consequences

- The ordinary boat scene does not trigger a full Chart Storage Element scan.
- Logs identify the stage, input attempt, postcondition, elapsed timeout and
  selected fallback.
- A pending DPB interaction is abandoned after its local timeout; subsequent
  cycles do not issue another interaction until the recovery state is reset.
- This policy does not add a terminal stop for a transient Chart Storage UI
  failure.

## Evidence

- Runtime log: `VOYAGE_PROGRESS_WAIT` followed by no Chart Storage action until
  the host aborted `OpenChartStorageTask.Run`.
- Source: `OpenChartStorageTask` and `ChartStorageUi.TryResolve`.
