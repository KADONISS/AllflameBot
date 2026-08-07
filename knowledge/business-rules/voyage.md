# Voyage Rules

## Purpose

Record confirmed Voyage workflow priorities.

## Audience

Agents changing Voyage planning, exploration or extraction.

## Update when

Update when source and runtime evidence confirm a changed Voyage rule.

## Current rules

- The Voyage Planner board is distinct from Voyage world terrain.
- Chart placement is verified after input; a requested click is not proof of
  placement.
- `ChartsRequiredForVoyage` controls when the planner is opened; once the
  planner is already open, a fresh snapshot with at least nine Charts permits
  placing the legal nine-Chart board.
- A planner-close recovery must verify that the planner is no longer visible
  before Chart Storage or Chart Area work can resume.
- The planner-close recovery sends Space only after a fresh open snapshot and
  verifies the postcondition before releasing `PrepareVoyageTask`.
- Allflame Capsule work has priority over ordinary frontier exploration.
- Lantern coverage and safe breadcrumbs are part of Voyage navigation state.
- Lantern placement is an attempt until a fresh counter or placed-lantern
  observation confirms the postcondition; the next placement may proceed after
  that observation instead of waiting on a fixed cooldown.
- Frontier, topology, detour and watchdog recovery share a bounded per-target
  budget; exhausting it rotates the strategy and does not stop the bot.
- Bathysphere call, wait and surface transfer are separate stages.
- A Bathysphere path failure must invalidate/rebuild movement context and may
  rotate bounded approach anchors derived from the observed portal position;
  it must not restart Chart Device recovery after Descend is confirmed.
- If Bathysphere approach recovery is exhausted, release the Chart Device
  request so the Chart Runner can resume instead of leaving a stale cycle
  active.
- Returning to the hub must reconcile pending surface loot and Voyage state.
