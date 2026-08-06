# Reliability Module Context

## Purpose

Context for watchdog, retries, route invalidation, stuck recovery and terminal
stop behavior.

## Audience

Agents investigating loops, stalls or automatic stops.

## Current source

`AllflameProgressWatchdog`, `AllflameMovementCoordinator`,
`AllflameStopController`, `StuckRecoveryTask` and `ObservationFallbackTask` are
in [`AllflameBot.cs`](../../AllflameBot.cs).

## Owns

- Progress evidence and retry accounting.
- Coalesced context-refresh requests for recoveries that can wait for the next
  task cycle.
- Voyage cycle timing, path-query and recovery-budget diagnostics.
- Movement ownership and priority leases.
- Recoverable runtime rebuild.
- Bounded Chart Storage UI open/resolve recovery and Voyage Stash fallback.
- Bounded Chart Device Descend confirmation recovery with Space, UI close,
  deferred fresh observation and Chart Runner handoff.
- Recording stop reasons.

## Required checks

Distinguish transient recovery from terminal stop. Prefer fresh observation,
reacquisition, path reload, target invalidation and retry-state reset. Do not
add an automatic stop merely because a loop is inconvenient to diagnose. Chart
Storage UI failures must remain bounded and return to the configured supply
fallback without converting a transient UI fault into a bot stop.
Normal Voyage exploration must use its explicit watchdog and bounded recovery
budget before yielding to the generic stuck-recovery task.
