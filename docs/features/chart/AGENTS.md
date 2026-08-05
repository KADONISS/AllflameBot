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
lantern/chest priority and extraction confirmation.

## Common mistakes

- Treating terrain row/column as a proven world coordinate.
- Letting exploration override capsule, chest or extraction work.
- Reusing a plan across an area-hash change.
- Declaring completion from target arrival without return/extraction evidence.

## Update when

Update when Chart task ownership, planner strategy or business priority changes.
