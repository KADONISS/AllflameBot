# Voyage Module Context

## Purpose

Context for Voyage planning, chart placement, Bathysphere entry, exploration,
capsules, lantern safety and surface transfer.

## Audience

Agents changing Voyage behavior.

## Current source

Voyage tasks, `VoyagePlanningUi`, `VoyageRunState` and voyage planners are in
[`AllflameBot.cs`](../../AllflameBot.cs).

## Owns

- Voyage planning UI and chart placement verification.
- Native Voyage Planner shadow reads and opt-in action canary.
- Planner-open target override and bounded Escape close verification.
- Voyage spawn, capsule anchors, lantern coverage and sector progress.
- Rolling frontier and topology routing share bounded path-query and recovery
  budgets; terrain snapshots are rebuilt only when their metadata signature
  changes.
- Bathysphere call/wait/extraction handoff.
- Bathysphere approach validates the current portal path, rotates bounded
  world-space approach anchors after repeated failures and releases the Chart
  Device request back to the Chart Runner only after recovery is exhausted.
- Surface transfer and return-to-hub state.

## Required checks

Verify one input action per cycle where required, fresh UI snapshots, inventory
delta, board slot postconditions, capsule priority and non-terminal recovery.
Native planner actions require consecutive agreement with the stable Element
snapshot; otherwise retain the stable Element/visual fallback.
After Chart Device Descend is confirmed, portal-entry timeout recovery must
reload/reacquire movement state without reopening the Chart Device transaction.
Normal frontier exploration owns an explicit progress watchdog and rotates its
target/recovery strategy after a bounded budget rather than accumulating
independent timeout chains.
