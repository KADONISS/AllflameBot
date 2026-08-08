# Bathysphere Stall Recovery Fixture

## Source assumptions

- Sanitized from the 2026-08-08 Voyage/Chart runner stall report.
- Runtime is DPB `net48` x64; exact client build and coordinates are intentionally
  omitted from this source fixture.
- This is a log-review fixture, not an automated live-client test.

## Failure sequence

```text
TASK_MANAGER_RUN_BEGIN|...
TASK_RUN_BEGIN|Task=LoadChartDeviceTask|...
MOVER_PATHFIND_BEGIN|Mode=Capsule/Direct|...
MOVER_PATHFIND_END|Found=False|...
MOVER_PATH_FAILED|HeldMovement=True|InputCleanup=True|Postcondition=ProcessHookKeyStatesCleared
BATHYSPHERE_PATH_RESET_BEGIN|...
BOT_INPUT_CLEANUP|Source=BathyspherePathResetBeforeReload|HookCleared=True|Postcondition=ProcessHookKeyStatesCleared
BATHYSPHERE_PATH_RELOAD_BEGIN|...
BATHYSPHERE_PATH_RELOAD_END|Reloaded=True|...
BOT_INPUT_CLEANUP|Source=BathyspherePathResetAfterReload|HookCleared=True|Postcondition=ProcessHookKeyStatesCleared
BATHYSPHERE_PATH_RESET|...
TASK_RUN_END|Task=LoadChartDeviceTask|...
TASK_MANAGER_RUN_END|...
```

## Expected recovery evidence

- If the owning interaction state machine exceeds its bounded wait,
  `CHART_STORAGE_INTERACTION_TIMEOUT` or the equivalent task-specific timeout
  is logged; for movement-finish, DPB's native timeout produces
  `ACTION_SYNC_TIMEOUT`. Input is cleared and a fresh context refresh is
  queued.
- If Bathysphere approach recovery exhausts its budget,
  `CHART_DEVICE_REQUEST_RELEASED` is followed by normal Chart Runner task
  activity; the bot does not stop solely for this transient path failure.
- An invalid `-1/-1` native counter produces `DEEPWATER_COUNTER_INVALID` or
  `DEEPWATER_COUNTER_SKIPPED` and leaves the text counter as the active
  fallback.
- A missing `TASK_RUN_END`, `CONTEXT_REFRESH_END`, or
  `BATHYSPHERE_PATH_RELOAD_END` identifies the operation that still blocks the
  cycle and is a residual live-client investigation signal.
