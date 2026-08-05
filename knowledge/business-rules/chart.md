# Chart Rules

## Purpose

Record confirmed Chart workflow priorities.

## Audience

Agents changing Chart transitions or exploration.

## Update when

Update when source and runtime evidence confirm a changed Chart rule.

## Current rules

- Chart Area entry creates a chart run state and records the immutable spawn when
  available.
- Exploration must not override safety, pending capsule work, chest/loot debt or
  extraction work.
- Terrain targets are an optional planner layer with hybrid fallback behavior.
- A target rejected by path validation must not be retried indefinitely without a
  route or planner state change.
- Chart completion requires a confirmed extraction/return transition, not only a
  movement target being reached.
