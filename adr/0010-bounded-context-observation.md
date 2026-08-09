# ADR 0010: Bounded Context Observation

- Status: Accepted
- Date: 2026-08-09
- Depends on: `adr/0002-ordered-observer-task-pipeline.md`,
  `adr/0003-layered-ui-fallbacks.md`,
  `adr/0006-bounded-chart-device-descend-recovery.md` and
  `adr/0007-cooperative-voyage-runtime-budget.md`

## Context

`AllflameContext.Refresh` performed external DPB object, inventory and UI
reads while holding its global lock. The refresh also traversed the complete
Element tree at Voyage/Chart Device frequency. A slow DPB read could therefore
stop the ordered task cycle before its watchdog or recovery task could run.

## Decision

- Use a single-flight refresh guard so a second refresh request is skipped and
  logged instead of waiting behind the active refresh.
- Keep the context lock only for refresh-request bookkeeping; never hold it
  across a DPB read.
- Prefer the verified native Chart Device and Capsule APIs during their Deepwater
  states, and use targeted Bathysphere/Capsule object lookup while waiting for
  extraction. A full object scan remains a bounded fallback.
- Bound the remaining visible-UI traversal by node count and cooperative
  elapsed-time checks. A slow or incomplete scan opens a short circuit breaker;
  cached text and native state continue to serve the task pipeline.
- Mark observation state as `Fresh`, `Stale` or `Unavailable` and record a
  refresh id and stage in boundary logs. These are recoverable observation
  states, not terminal bot-stop conditions.

Background execution of arbitrary DPB reads is deliberately not introduced
until the external DPB assembly proves those calls are thread-safe. A timeout
wrapper around a thread-affine or non-cancellable DPB call would add a second
failure mode rather than make the bot safer.

## Consequences

- Chart Device and Bathysphere workflows no longer require a full raw UI-tree
  scan on every refresh cycle.
- A cached visible-UI result can remain stale briefly, but consequential native
  actions still require their own postcondition and existing Element fallback.
- Logs identify the refresh id, current stage, bounded UI scan and circuit
  breaker state when observation degrades.
- A single external getter can still block its caller if DPB itself blocks inside
  that getter; the stage boundary now identifies that remaining external limit.

## Evidence

- Latest paired sessions ended after `CONTEXT_REFRESH_BEGIN` without a matching
  end event while waiting for Bathysphere or confirming Chart Device Descend.
- `AllflameBot.cs` previously held `AllflameContext.Sync` across
  `SnapshotObjects`, `SnapshotInventory` and `CaptureVisibleUiText`.
