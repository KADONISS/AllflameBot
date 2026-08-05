# Workflow: Planner Change

## Purpose

Safely change Chart or Voyage target generation.

## Audience

Agents changing terrain, topology, frontier or movement selection.

## Update when

Update when planner ownership, coordinate semantics or fallback order changes.

## Steps

1. Identify planner and area hash lifecycle.
2. State coordinate space, origin, scale and layer assumptions.
3. Check target path validation and blocked-target invalidation.
4. Preserve capsule/extraction/loot movement ownership.
5. Compare primary planner and fallback behavior.
6. Add terrain/planner fixture or runtime diagnostic.
7. Update map-model and ADR documentation.
