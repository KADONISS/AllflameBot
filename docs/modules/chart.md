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

## Required checks

Validate area hash, spawn coordinates, terrain dimensions, coordinate transform,
path existence, lantern preservation and capsule/extraction preemption.
