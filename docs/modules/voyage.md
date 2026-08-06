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
- Voyage spawn, capsule anchors, lantern coverage and sector progress.
- Bathysphere call/wait/extraction handoff.
- Surface transfer and return-to-hub state.

## Required checks

Verify one input action per cycle where required, fresh UI snapshots, inventory
delta, board slot postconditions, capsule priority and non-terminal recovery.
Native planner actions require consecutive agreement with the stable Element
snapshot; otherwise retain the stable Element/visual fallback.
