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
- Bounded post-Descend transition recovery that treats a successful native
  Descend as accepted, defers Chart Device UI reads and uses area/capsule
  postconditions with a Chart Runner handoff after exhaustion.
- Bounded post-Descend area-transition recovery: one Space after 10 seconds,
  one delayed Bathysphere retry, then Chart Device reacquisition with a fresh
  transaction and a finite reopen budget.
- Bounded Bathysphere approach recovery with path reload, mover reset, live
  portal reacquisition and Chart Runner handoff after exhaustion.
- Lifecycle stop and deinitialization release held Allflame Mover input and
  clear process-hook key states before task teardown.
- An independent runtime heartbeat detects a TaskManager/native cycle stall;
  host-thread recovery clears input and stale Chart Device/transition state,
  sends Space when the Chart Device transaction is active, returns to Chart
  Runner, and uses a bounded runtime rebuild fallback per `AreaHash`.
- DPB interaction and `FinishCurrentAction` waits use one DPB coroutine task at
  a time; task-specific state machines provide cross-tick timeouts where
  needed, release held input, queue a fresh context snapshot and return
  control to the ordered task loop when an action does not complete.
- Valerie Chart identification verifies inventory progress after input; a
  bounded no-progress sequence records the live player position, blacklists a
  five-unit approach radius for the current `AreaHash` and replans without
  adding a terminal stop.
- Context observation uses a single-flight coordinator, stage diagnostics,
  targeted Deepwater object reads and a bounded visible-UI circuit breaker so a
  recoverable observation slowdown does not monopolize the task cycle. Global
  Element traversal is disabled in Deepwater encounter areas; native state and
  action-scoped fallbacks remain available.
- Map-entry utility-flask protection uses a five-second cooldown, transition
  guard, input cleanup and a per-area circuit breaker for slow or failed native
  calls so movement can continue without a terminal stop.
- Chart zero-Lantern recovery that cancels unopened chest routes, preserves a
  confirmed committed chest loot session and waits for loot before return.
- Recording stop reasons.

## Required checks

Distinguish transient recovery from terminal stop. Prefer fresh observation,
reacquisition, path reload, target invalidation and retry-state reset. Do not
add an automatic stop merely because a loop is inconvenient to diagnose. Chart
Storage UI failures must remain bounded and return to the configured supply
fallback without converting a transient UI fault into a bot stop.
Interaction and movement-finish timeouts must be recoverable input cleanup
events, not terminal bot stops; the timeout and cleanup postcondition must be
logged.
An accepted Valerie interaction is not an identification postcondition until
the unidentified inventory count decreases. A no-progress approach block must
be area-scoped, use the live world position and preserve the existing broader
Chart Device recovery blacklist.
Once Chart Device Descend is accepted, a Bathysphere path or portal-entry
timeout is an extraction recovery, not a new Chart Device transaction; the
recovery must preserve capsule priority, avoid native Chart Device UI probes
and release the Chart request only after its bounded budget is exhausted.
Utility-flask native calls must be skipped during the first five seconds of a
new encounter area and while an area transition is pending. Log the call
duration and release movement/process-hook input before and after the call; a
slow or failed utility call may disable utility flasks for that area but must
not stop the bot or disable emergency Life Flask use.
If a global Element scan is slow or incomplete, disable that scan until the
current `AreaHash` changes. Do not infer current UI flags from cached text
after a skipped or incomplete scan; require native state or an action-scoped
fallback instead.
Normal Voyage exploration must use its explicit watchdog and bounded recovery
budget before yielding to the generic stuck-recovery task. Chart zero-Lantern
loot preservation applies only to Chart Areas; Voyage extraction priorities are
unchanged.
