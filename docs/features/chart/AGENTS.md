# Chart Feature Agent Guide

## Purpose

Guide changes to Chart transitions, terrain exploration, lanterns, chests and
extraction.

## Required reading

[`docs/modules/chart.md`](../../modules/chart.md),
[`knowledge/business-rules/chart.md`](../../../knowledge/business-rules/chart.md),
[`knowledge/map-model.md`](../../../knowledge/map-model.md),
[`rules/navigation.md`](../../../rules/navigation.md) and
[`workflows/planner-change.md`](../../../workflows/planner-change.md).

## Ownership

Preserve Chart run state, immutable spawn, terrain/topology/hybrid fallback,
lantern/chest priority, committed chest-loot handoff and extraction
confirmation.

## Common mistakes

- Treating terrain row/column as a proven world coordinate.
- Letting exploration override capsule, chest or extraction work.
- Opening a new chest after a confirmed `0/x` Lantern counter, or abandoning
  loot from a chest whose open was already confirmed before exhaustion.
- Reusing a plan across an area-hash change.
- Declaring completion from target arrival without return/extraction evidence.

## Update when

Update when Chart task ownership, planner strategy or business priority changes.
