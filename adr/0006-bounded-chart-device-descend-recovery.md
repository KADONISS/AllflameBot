# ADR 0006: Bounded Chart Device Descend Recovery

- Status: Accepted (documented current contract)
- Date: 2026-08-07
- Depends on: `adr/0002-ordered-observer-task-pipeline.md`,
  `adr/0003-layered-ui-fallbacks.md`,
  `adr/0005-bounded-chart-storage-recovery.md`

## Context

The Chart Device can accept a Descend input while its native UI state remains
stale or unavailable. Reading that state immediately after the action can
block the host coroutine inside DPB, preventing the task-level timeout and
watchdog recovery from running.

## Decision

Treat a successful Descend return value as an accepted action and enter a
post-Descend state. Do not read Chart Device UI state while waiting for the
area hash to change or the Bathysphere to appear. Use the existing bounded
portal-appearance and portal-entry recovery budgets; if they are exhausted,
clear the Chart Device request and return control to the existing Chart Runner
pipeline without adding a terminal stop.

Record begin/end boundaries for TaskManager cycles, context refresh stages and
remaining native UI probes so a future stall can be attributed to a synchronous
external operation. The runtime watchdog may queue host recovery, but its timer
thread must not call DPB UI or input APIs.

## Consequences

- A native UI getter cannot block the post-Descend task cycle.
- Area transition and Bathysphere object state become the postconditions for
  extraction rather than UI closure.
- Input state is explicitly cleared by the existing bounded transition and
  runtime recovery paths.
- Fresh context verification remains cooperative and is not forced inside the
  post-Descend state.
- A pre-Descend DPB getter can still block its caller; probe begin/end logs
  identify that remaining external limitation.

## Evidence

- Runtime log: native `Descend()` returned `True`, followed by a runtime stall
  before any post-Descend transition evidence was logged.
- Source: `EnterBathysphereTask`, `AllflameContext.Refresh` and
  `NativeDeepwaterChartDeviceUi.Descend`.
