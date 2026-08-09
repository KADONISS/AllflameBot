# Context Refresh Stall Recovery Fixture

## Source assumptions

- Sanitized from the paired 2026-08-09 sessions on DPB `0.3.29.31` and
  AllflameBot `0.23.166`.
- Runtime is .NET Framework 4.8 x64; this is a log-review fixture, not an
  automated live-client test.

## Failure sequence before the fix

```text
CONTEXT_REFRESH_BEGIN|...
CONTEXT_REFRESH_STAGE|Stage=VisibleUi|State=Begin
TASK_MANAGER_RUN_END|...
```

The final refresh boundary had no matching `CONTEXT_REFRESH_END`, and the
process was later stopped manually. The failure was a synchronous observation
stall, not a recorded coroutine fault or terminal recovery stop.

## Expected evidence after the fix

- Chart Device refreshes log
  `CONTEXT_REFRESH_STAGE|State=Skipped|Reason=NativeStateAvailable` while the
  native Chart Device API owns the state.
- Voyage/Bathysphere refreshes use native Capsule state and targeted
  `CONTEXT_OBJECT_TARGETED` lookup before falling back to a full object scan.
- A bounded visible-UI traversal emits `UI_SCAN_BOUNDED` and
  `CONTEXT_REFRESH_UI_CIRCUIT` when its budget is exceeded, then later refreshes
  continue with cached/native state.
- Refresh boundary records include `RefreshId`, `Stage` and
  `ObservationStatus=Fresh|Stale|Unavailable`.
- A transient observation failure does not emit a new terminal bot stop; the
  ordered task pipeline and existing task-specific postcondition recovery keep
  running.

## Validation limit

The repository has no automated test project. Build validation and live two-bot
smoke/soak runs are required before release.
