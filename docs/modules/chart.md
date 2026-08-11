# Chart Module Context

## Purpose

Context for Chart Area transition, planning, terrain exploration, lanterns,
chests, loot and extraction.

## Audience

Agents changing Chart behavior or map planning.

## Current source

Chart-related tasks and planners are in [`AllflameBot.cs`](../../AllflameBot.cs).
Chart settings are in [`AllflameSettings.cs`](../../AllflameSettings.cs).

## Owns

- Chart request and Chart Area lifecycle.
- Immutable spawn and exploration progress.
- Terrain/topology/hybrid target selection.
- Lantern and chest handoff priorities.
- Native Chart Device state reads with stable UI fallback.
- Valerie identification postconditions, bounded obstacle recovery and
  area-scoped approach invalidation.
- Confirmed zero-Lantern gating for new chest work and the handoff of committed
  chest loot to the return flow.

## Required checks

Validate area hash, spawn coordinates, terrain dimensions, coordinate transform,
path existence, lantern preservation, zero-counter chest gating and
capsule/extraction preemption.
