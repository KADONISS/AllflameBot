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
- Native DPB World Item pickup through `Coroutines.Loot` for ground loot and
  Sulphur rewards, with `ShouldMove` delegated to the movement coordinator and
  `InteractWith` retained as the unavailable-API fallback.

## Required checks

Preserve safety/capsule/extraction priority, verify inventory deltas, account for
Message in a Bottle cursor behavior and avoid declaring completion from a stale
inventory snapshot. Keep committed Chart chest loot ahead of Bathysphere return
until its eligible drops are resolved. Do not replace filtered
`FastMoveReward()` with native `TakeAll()`.

For World Items, a native `None` result and a timeout are followed by the
existing inventory/world-item postcondition check. A typed native failure is
refreshed and retried through the task cooldowns; it does not create a new stop
path.
