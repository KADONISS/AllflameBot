# Runtime Stall Watchdog Recovery

## Trigger

- A `BOT_RUNTIME_HEARTBEAT` is absent for at least 10 seconds while the bot is
  started.
- The progress context may be `LoadChartDeviceTask` after native Chart Device
  `Descend()`.

## Expected recovery

1. The timer logs `BOT_RUNTIME_STALL` and only queues a host-thread recovery.
2. The next host `Tick()` releases mover/process-hook input, sends Space when a
   Chart Device transaction is active, clears transition/UI/progress state,
   releases the Chart Device request and queues a fresh snapshot.
3. If no heartbeat returns within 5 seconds, the runtime is rebuilt once while
   preserving the per-area recovery budget.
4. After two attempts for the same `AreaHash`, the request is released and
   further watchdog retries are suppressed until the area changes.

## Safety

- The timer callback never calls DPB UI, input or context APIs.
- A normal heartbeat clears a queued stall before recovery is issued.
