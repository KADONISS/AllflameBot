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

## Post-Descend native UI stall regression

Before the post-Descend quarantine, a successful native action could be
followed by a blocking Chart Device state read:

```text
CHART_DEVICE_NATIVE_CALL_END|Operation=Descend|Result=True
CHART_DEVICE_NATIVE_PROBE_BEGIN|Operation=IsOpened
```

The missing probe end boundary identified a synchronous DPB getter that
prevented the next task heartbeat and host recovery from running. The current
contract accepts the action result and defers Chart Device UI reads:

```text
CHART_DEVICE_NATIVE_CALL_END|Operation=Descend|Result=True
CHART_DEVICE_POST_DESCEND_STATE|State=AwaitingTransition|Evidence=DescendAccepted|Postcondition=AreaHashOrBathysphere
CHART_DEVICE_TRANSITION_HEARTBEAT|UiObservation=DeferredPostDescend|...
BATHYSPHERE_PORTAL_READY|...
```

No `CHART_DEVICE_NATIVE_PROBE_BEGIN` or stable-Element UI probe is expected
between the accepted Descend and the area/capsule postcondition. A missing
`CHART_DEVICE_POST_DESCEND_STATE` after `Descend` remains a regression signal.

## Chart Device reopen sequence

```text
CHART_AREA_TRANSITION_ARMED|Interactions=1|TimeoutSeconds=30
CHART_AREA_TRANSITION_SPACE_INPUT|Reason=NoAreaTransitionAfter10Seconds|InputSent=True|Postcondition=DeferredFreshSnapshot
BATHYSPHERE_ENTER|Mode=BoundedTransitionRetry|Interaction=2|Success=True
CHART_AREA_TRANSITION_RECOVERY_EXHAUSTED|Reason=Retry2NoAreaTransition|Recovery=1|Action=ReopenChartDevice
CHART_DEVICE_REOPEN_RECOVERY_WAIT|UiOpen=False|Action=PlanVoyageReacquire
CHART_DEVICE_OPEN|Source=Detected|RecoveryRequested=True
CHART_DEVICE_REOPEN_RECOVERY_CONSUMED|Postcondition=ReacquireChartDeviceTransaction
CHART_DEVICE_REOPEN_RECOVERY_TRANSACTION_RESET|Action=LoadAndDescend|Postcondition=FreshChartDeviceTransaction
CHART_DEVICE_LOAD_CONFIRMED|InventoryChartsBeforeLoad=1|InventoryChartDelta=1|DeviceCharts=1|DeviceChartPostcondition=True
CHART_DEVICE_DESCEND_CALL_END|InputSent=True
CHART_AREA_TRANSITION_READY|Interactions=1
```

The second Chart Device transaction must not reuse `_descendConfirmed` or the
old portal id. If the native device already contains one item, the recovery
must not issue a second Ctrl-click; it waits for the single-item postcondition
and proceeds to Descend. If the bounded Chart Device recovery budget is
exhausted, the expected handoff is:

```text
CHART_DEVICE_RECOVERY_EXHAUSTED|Action=ReleaseChartDeviceAndResumeChartRunner
CHART_DEVICE_REQUEST_RELEASED|ChartSupplyChecksReset=True|Action=ResumeChartRunner
```

## Expected recovery evidence

After the initial Bathysphere input is accepted, extraction owns the
transition until an area-hash postcondition is observed. The owning task sends
one Space after 10 seconds, retries the Bathysphere after the five-second
cooldown, and does not let the Chart Device task compete with that recovery:

```text
VOYAGE_STAGE|Stage=9|Action=Extract|Success=True|...
BATHYSPHERE_TRANSITION_ARMED|Mode=VoyageExtraction|...
BATHYSPHERE_TRANSITION_WAIT|Mode=VoyageExtraction|...
BATHYSPHERE_SPACE_INPUT|Reason=NoAreaTransitionAfter10Seconds|InputSent=True|Postcondition=DeferredFreshSnapshot
BATHYSPHERE_SPACE_RECOVERY|Mode=VoyageExtraction|Interactions=1|RetryAfterSeconds=5|Postcondition=DeferredFreshSnapshot
BATHYSPHERE_ENTER|Mode=VoyageExtractionRetry|Interaction=2|Success=True
BATHYSPHERE_TRANSITION_READY|Mode=VoyageExtraction|...
```

If the second interaction still has no area-transition postcondition, the
voyage call is released and the normal capsule-rescue/runner state can resume
without a new automatic stop:

```text
BATHYSPHERE_EXTRACTION_RECOVERY_EXHAUSTED|Action=ReleaseCallAndResumeCapsuleRescue
CHART_DEVICE_REQUEST_RELEASED|Reason=BathysphereExtractionRecoveryExhausted|Action=ResumeChartRunner
```

- If the owning interaction state machine exceeds its bounded wait,
  `CHART_STORAGE_INTERACTION_TIMEOUT` or the equivalent task-specific timeout
  is logged; for movement-finish, DPB's native timeout produces
  `ACTION_SYNC_TIMEOUT`. Input is cleared and a fresh context refresh is
  queued.
- If Bathysphere approach recovery exhausts its budget,
  `CHART_DEVICE_REQUEST_RELEASED` is followed by normal Chart Runner task
  activity; the bot does not stop solely for this transient path failure.
- If three in-range Bathysphere inputs are rejected, the extraction task logs
  `BATHYSPHERE_EXTRACT_GIVE_UP`, releases the call and process-hook input, and
  returns to capsule rescue instead of calling `AllflameStopController.Stop`.
- A stalled area transition sends one Space after 10 seconds, waits 5 seconds
  before interaction #2, and reopens the Chart Device only after the second
  interaction still has no area-transition postcondition.
- A Chart Device reopen keeps the recovery request until the new UI is
  detected, resets the old transaction, verifies one loaded chart and then
  issues Descend. Repeated failures are bounded and return to Chart Runner.
- An invalid `-1/-1` native counter produces `DEEPWATER_COUNTER_INVALID` or
  `DEEPWATER_COUNTER_SKIPPED` and leaves the text counter as the active
  fallback.
- A missing `TASK_RUN_END`, `CONTEXT_REFRESH_END`, or
  `BATHYSPHERE_PATH_RELOAD_END` identifies the operation that still blocks the
  cycle and is a residual live-client investigation signal.
