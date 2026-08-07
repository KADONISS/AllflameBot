# Loot Module Context

## Purpose

Context for chest opening, ground loot, item filtering, surface collection and
stash transfer.

## Audience

Agents changing loot, inventory or transfer behavior.

## Current source

Loot tasks and `AllflameLootState` are in [`AllflameBot.cs`](../../AllflameBot.cs).
Price/filter support is in [`AllflamePrices.cs`](../../AllflamePrices.cs) and
[`AllflameSettings.cs`](../../AllflameSettings.cs).

## Owns

- Chest priority and open-session debt.
- Committed Chart chest loot that must finish across a confirmed zero-Lantern
  transition, without admitting a new chest.
- Ground-item acceptance/rejection and pickup verification.
- Inventory capacity, surface loot and stash handoff.
- Before Voyage, the stash policy transfers eligible non-Chart items while
  retaining Charts in inventory; reserved Message in a Bottle items keep their
  existing processing exception.
- Native Allflame Loot UI state with filtered reward transfer.

## Required checks

Preserve safety/capsule/extraction priority, verify inventory deltas, account for
Message in a Bottle cursor behavior and avoid declaring completion from a stale
inventory snapshot. Keep committed Chart chest loot ahead of Bathysphere return
until its eligible drops are resolved. Do not replace filtered
`FastMoveReward()` with native `TakeAll()`.
