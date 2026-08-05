# ADR 0004: Hybrid Terrain Planning

- Status: Accepted (documented current implementation)
- Date: 2026-08-06
- Depends on: `knowledge/map-model.md`, `architecture/boundaries/map-coordinate.md`

## Context

DPB exposes terrain/cache and pathfinding data, but runtime availability and
coordinate semantics must be validated. Existing layout/topology/frontier logic
is valuable fallback behavior.

## Decision

Use terrain-derived targets when the snapshot is valid, retain topology/layout
or rolling-frontier strategies as fallbacks, and log which planner owns a target.

## Consequences

- Terrain integration must not remove existing progress paths without evidence.
- Coordinate origin and scale need explicit runtime diagnostics.
- Chart and Voyage planners may use different target-generation strategies until
  their contracts are proven equivalent.
