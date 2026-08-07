# Loot Feature Agent Guide

## Purpose

Guide changes to chest, ground loot, item filtering, surface collection and
stash transfer.

## Required reading

[`docs/modules/loot.md`](../../modules/loot.md),
[`knowledge/business-rules/loot.md`](../../../knowledge/business-rules/loot.md),
[`rules/testing.md`](../../../rules/testing.md) and
[`workflows/feature-change.md`](../../../workflows/feature-change.md).

## Ownership

Preserve chest priority, committed loot debt, inventory postconditions, cursor
reward handling and stash reconciliation.

## Common mistakes

- Treating an item disappearance as proof of successful pickup or transfer.
- Ignoring inventory capacity during a multi-stage reward flow.
- Letting loot work override safety or extraction.
- Clearing a confirmed Chart chest loot session merely because the Lantern
  counter reached `0/x`.
- Pricing an unresolved unique as a confirmed low-value item.

## Update when

Update when loot policy, item filters, transfer behavior or pricing semantics change.
