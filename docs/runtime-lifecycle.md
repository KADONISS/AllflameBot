# Runtime Lifecycle

## Purpose

Give operators and agents a concise lifecycle reference.

## Audience

Anyone diagnosing a stalled task or changing lifecycle code.

## Update when

Update when startup, shutdown, coroutine recovery, task order or phase mapping
changes.

## Depends on

[`architecture/runtime-flow.md`](../architecture/runtime-flow.md),
[`AllflameBot.cs`](../AllflameBot.cs) and
[`docs/modules/reliability.md`](modules/reliability.md).

## Callbacks

- `Initialize`: logging, settings, prices and task registration.
- `Start`: state reset, session start, mover selection, process hook and main
  coroutine.
- `Tick`: task-manager tick, notification tick and coroutine resume. Notification
  tick schedules bounded periodic dashboard work and price/supply checks; it
  does not perform network I/O synchronously in the task cycle.
- `Logic`: forwards logic requests to enabled tasks.
- `Message`: forwards messages; `allflame_bot.snapshot` requests a main-cycle
  context snapshot.
- `Stop`: stops tasks, ends session and optionally closes the game.
- `Deinitialize`: invokes stop and logs shutdown.

## Important distinction

The main loop catches and recovers ordinary exceptions. A stop request is a
separate terminal control path. A log entry saying recovery occurred does not
prove that no later stop path was reached.
