# Recovery Rules

## Purpose

Record the intended distinction between recoverable faults and terminal stops.

## Audience

Agents changing watchdog, retry or stop behavior.

## Update when

Update when a recovery contract or terminal condition is intentionally changed.

## Current rules

- Prefer refreshing observation, reacquiring UI/object state, reloading path data,
  invalidating a target or rebuilding the coroutine runtime.
- A repeated failure is evidence to change state or strategy, not automatically a
  reason to stop.
- Existing `AllflameStopController.Stop` call sites are legacy terminal paths and
  must be audited before modification.
- Recovery must leave input and movement ownership in a known state.
- Every recovery action needs a structured log with reason and next action.
