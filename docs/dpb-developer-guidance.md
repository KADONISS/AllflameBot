# DPB Developer Guidance

## Purpose

Keep the concise DreamPoeBot integration rules that are useful while writing
new AllflameBot logic.

## Audience

Developers and AI agents working at the DPB boundary.

## Update when

Update when the matching DPB developer notes or referenced APIs change.

## Depends on

[`docs/dpb-compatibility.md`](dpb-compatibility.md),
[`architecture/boundaries/dpb-api.md`](../architecture/boundaries/dpb-api.md) and
the actual DPB assembly used for the build.

## Guidance

- Keep DPB message handlers short; do not perform blocking or expensive work on
  the main coroutine thread.
- Treat message ids and custom input/output types as integration contracts.
- Process one bounded action per bot cycle and yield back to the task manager.
- Reacquire nearby objects after meaningful movement; cached object references can
  become stale.
- Cache only the small set of object properties needed by the active task and
  limit refresh cadence.
- Avoid repeated live-memory property reads and heavy LINQ chains in dense areas.
- Treat asynchronous UI actions as incomplete until the selected value or result
  is freshly observed.
- Bound place, cancel, retrieve, transfer and interaction operations and release
  held input on every failure path.

These rules describe integration discipline; exact API availability must still
be checked against the deployed DPB build.
