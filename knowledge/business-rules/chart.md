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
- A confirmed `0/x` Lantern counter blocks selecting, moving to or opening a
  new Chart chest.
- A Chart chest opened successfully while the confirmed counter is positive
  creates committed loot debt; the runner finishes that chest's eligible drops
  even if the counter reaches `0/x` during the loot session.
- Once committed chest loot is complete, the runner resumes the existing
  Bathysphere/extraction return flow instead of exploring or opening another
  chest.
- Chart completion requires a confirmed extraction/return transition, not only a
  movement target being reached.
