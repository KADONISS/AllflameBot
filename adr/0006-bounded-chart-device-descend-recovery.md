# ADR 0006: Bounded Chart Device Descend Recovery

- Status: Accepted (documented current contract)
- Date: 2026-08-07
- Depends on: `adr/0002-ordered-observer-task-pipeline.md`,
  `adr/0003-layered-ui-fallbacks.md`,
  `adr/0005-bounded-chart-storage-recovery.md`

## Context

The Chart Device could accept a Descend input without producing a later UI
closure or transition observation. The task-level timeout could not recover
when the next cycle was delayed, and the old recovery path did not explicitly
release input or close the blocking UI before reacquisition.

## Decision

Treat an unconfirmed Descend as a bounded cooperative recovery. After three
seconds without confirmation, send Space, close blocking windows, queue a
fresh context snapshot for the next task cycle and reacquire the Chart Device.
Allow two confirmation recoveries per transaction. When those recoveries are
exhausted, clear the Chart Device request and return control to the existing
Chart Runner pipeline without adding a terminal stop.

Record begin/end boundaries for TaskManager cycles, context refresh stages and
native Descend calls so a future stall can be attributed to a synchronous
external operation.

## Consequences

- A still-open Chart Device is no longer retried blindly after the confirmation
  deadline.
- Input state is explicitly cleared during recovery.
- Fresh context verification remains cooperative and is not forced inside the
  recovery method.
- This policy does not guarantee recovery from a host thread that is already
  blocked inside an external DPB call; the new boundary logs identify that
  operation for the next fix.

## Evidence

- Runtime log: `CHART_DEVICE_DESCEND` followed by no confirmation or heartbeat
  before `MAIN_LOOP_FAILED|ThreadAbortException`.
- Source: `EnterBathysphereTask`, `AllflameContext.Refresh` and
  `NativeDeepwaterChartDeviceUi.Descend`.
