# ADR 0009: Stop-Owned Input Cleanup

## Status

Accepted

## Date

2026-08-07

## Context

The mover can leave a held movement action active while the bot coroutine or
task manager is stopped. Clearing the mover's flag without clearing the
process-hook state leaves the game receiving stale input, including after the
bot has stopped.

## Decision

- Release the Allflame Mover's held movement input during bot stop, runtime
  rebuild and mover deinitialization.
- Clear process-hook key states after owned cleanup and log whether the
  postcondition was achieved.
- Use Space for bounded planner recovery close actions and clear key states in
  a finally-equivalent cleanup path.

## Consequences

- The game should accept manual mouse and keyboard input after the bot stops.
- Cleanup is idempotent and does not add a new terminal stop path.
- A cleanup failure is logged as an unavailable postcondition for diagnosis.

## Alternatives considered

- Only reset the mover's boolean flag: rejected because it does not release
  the external process-hook state.
- Send Escape for every blocking window: rejected because it is broader than
  the verified planner recovery action and can affect unrelated UI state.

## Evidence

- Source: `AllflameMover.ReleaseMovementInput`, `AllflameBot.Stop`,
  `AllflameBot.CleanupStaleRuntimeForRestart` and planner recovery methods.
- Runtime markers: `BOT_INPUT_CLEANUP`, `MOVER_INPUT_CLEANUP` and
  `VOYAGE_PLANNER_CLOSE`.
