# Voyage Coroutine Single-Task Fixture

## Source assumptions

- Sanitized from the 2026-08-08 `0.23.164` Voyage/Chart runner regression.
- Runtime is DPB `net48` x64; exact client coordinates are intentionally
  omitted.
- This is a source-invariant and log-review fixture, not a live-client test.

## Regression symptom

```text
ACTION_SYNC_FAILED|Task=PrepareVoyageTask|Target=Valerie Voyage|Error=InvalidOperationException:Cannot obtain multiple coroutine tasks in a single coroutine tick.
BOT_COROUTINE_FAULT|Type=CoroutineUnhandledException|Context=ActiveTask=PrepareVoyageTask
```

The regression was caused by starting a DPB coroutine action and a second
`LokiCoroutine.Sleep` timeout before awaiting either task.

## Fixed invariant

- `InteractWithRecovery` awaits `Coroutines.InteractWith` directly.
- `FinishMovementBeforeInteraction` awaits `Coroutines.FinishCurrentAction`
  directly after configuring its native timeout.
- Cross-tick timeout ownership remains in task state machines such as
  `OpenChartStorageTask`.
- A rejected interaction or synchronization exception releases input and
  queues a fresh context snapshot.

## Expected evidence

- No `Cannot obtain multiple coroutine tasks in a single coroutine tick`.
- With fewer than nine charts, the planner closes and emits
  `VOYAGE_PROGRESS_WAIT` before Chart runner work resumes.
- With nine charts, board placement can proceed one action per fresh snapshot
  until `BeginVoyage` is confirmed.
