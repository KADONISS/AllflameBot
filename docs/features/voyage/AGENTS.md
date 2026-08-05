# Voyage Feature Agent Guide

## Purpose

Guide changes to Voyage planning, chart placement, capsule traversal, lantern
safety and Bathysphere extraction.

## Required reading

[`docs/modules/voyage.md`](../../modules/voyage.md),
[`knowledge/business-rules/voyage.md`](../../../knowledge/business-rules/voyage.md),
[`rules/ui-interaction.md`](../../../rules/ui-interaction.md),
[`rules/navigation.md`](../../../rules/navigation.md) and
[`workflows/ui-change.md`](../../../workflows/ui-change.md).

## Ownership

Preserve one-action/fresh-snapshot placement verification, capsule priority,
lantern coverage, Bathysphere stages and surface-loot reconciliation.

## Common mistakes

- Confusing the Voyage Planner board with Voyage world terrain.
- Trusting input-sent instead of board/inventory postconditions.
- Allowing frontier exploration to override capsule or extraction movement.
- Adding a stop instead of restoring observation or UI state.

## Update when

Update when Voyage task ownership, UI adapter order or extraction policy changes.
