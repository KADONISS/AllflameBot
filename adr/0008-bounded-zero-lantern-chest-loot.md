# ADR 0008: Bounded Zero-Lantern Chart Chest Loot

## Status

Accepted

## Date

2026-08-07

## Context

Chart Lantern exhaustion previously had one safety outcome: cancel chest and
ground-loot work and return to the Bathysphere. That can abandon drops from a
chest whose open was already accepted, while allowing the chest route and loot
tasks to compete with the mandatory return decision.

## Decision

- A Chart chest open is eligible for commitment only when the confirmed Lantern
  counter is positive at the start of the open interaction.
- A confirmed open records the chest loot session and its area hash. If the
  counter later becomes confirmed `0/x`, zero-Lantern recovery cancels pending
  unopened chest routes but preserves this committed loot debt.
- The chest movement and open tasks reject new Chart chest work at confirmed
  `0/x`. The ground-loot task continues to use the existing filter, inventory
  and pickup postconditions for the committed chest.
- After the committed loot session completes, the existing Chart
  Bathysphere/extraction flow owns the next action. Voyage zero-Lantern
  extraction remains unchanged.

## Consequences

- Drops from an already-opened Chart chest are not discarded solely because the
  Lantern counter reached zero.
- No new chest is opened after confirmed Lantern exhaustion.
- The committed session is scoped to the original Chart area and is cleared by
  normal loot completion or an area transition.
- Existing safety, inventory capacity, item-filter and bounded retry behavior
  remains in force; this decision does not add an unbounded wait or a new stop
  path.

## Alternatives considered

- Immediately cancel all loot at `0/x`: rejected because it loses rewards from a
  chest already opened successfully.
- Continue opening every known chest at `0/x`: rejected because it violates the
  Lantern safety boundary and can delay extraction.
- Add a separate Chart return implementation: rejected because the existing
  Bathysphere flow already owns extraction and has the required postconditions.

## Evidence

- Source: `AllflameLootState`, `ZeroLanternSafety`,
  `MoveToDeepwaterChestTask`, `OpenDeepwaterChestsTask`,
  `LootGroundItemsTask` and `ReturnToHubTask` in `AllflameBot.cs`.
- Runtime markers: `CHEST_LOOT_COMMITTED`, `CHEST_ZERO_LANTERN_LOOT` and
  `CHART_RETURN_WAIT`.
