# Loot Rules

## Purpose

Record confirmed loot and inventory rules.

## Audience

Agents changing chest, pickup or stash behavior.

## Update when

Update when item acceptance, transfer or inventory postconditions change.

## Current rules

- Chest priorities come from settings and are resolved before normal exploration.
- Loot actions must account for inventory capacity and fresh inventory state.
- Message in a Bottle has cursor/reward placement state that cannot be inferred
  only from the bottle item disappearing.
- Valuable or uncertain rewards must not be treated as confirmed low value.
- A confirmed Chart chest open while Lanterns remain creates a committed loot
  session. The session survives a later confirmed `0/x` counter, while pending
  and new chest routes are cancelled or blocked.
- Committed chest loot still requires the normal item-filter, inventory and
  pickup postconditions before the return flow can proceed.
- Stash and surface transfer require transfer and reconciliation evidence.
