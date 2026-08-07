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
- Bounded Bathysphere approach recovery with path reload, mover reset, live
  portal reacquisition and Chart Runner handoff after exhaustion.
- Lifecycle stop and deinitialization release held Allflame Mover input and
  clear process-hook key states before task teardown.
- Chart zero-Lantern recovery that cancels unopened chest routes, preserves a
  confirmed committed chest loot session and waits for loot before return.
- Recording stop reasons.

## Required checks

Distinguish transient recovery from terminal stop. Prefer fresh observation,
reacquisition, path reload, target invalidation and retry-state reset. Do not
add an automatic stop merely because a loop is inconvenient to diagnose. Chart
Storage UI failures must remain bounded and return to the configured supply
fallback without converting a transient UI fault into a bot stop.
Once Chart Device Descend is confirmed, a Bathysphere path or portal-entry
timeout is an extraction recovery, not a new Chart Device transaction; the
recovery must preserve capsule priority and release the Chart request only
after its bounded budget is exhausted.
Normal Voyage exploration must use its explicit watchdog and bounded recovery
budget before yielding to the generic stuck-recovery task. Chart zero-Lantern
loot preservation applies only to Chart Areas; Voyage extraction priorities are
unchanged.
