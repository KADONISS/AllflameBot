# Map and Coordinate Boundary

## Purpose

Make map data, navigation coordinates and transformations explicit.

## Audience

Agents changing Chart or Voyage exploration.

## Update when

Update when terrain buffers, world origin, tile scale, layer semantics or
planner target generation changes.

## Depends on

[`knowledge/map-model.md`](../../knowledge/map-model.md),
[`rules/navigation.md`](../../rules/navigation.md) and
[`docs/modules/chart.md`](../../docs/modules/chart.md).

## Current contract

`AreaMapSnapshot` records area hash/id, rows, columns, nav-cell size, terrain
data, fly data, jump mask and walkability. The current Chart planner constructs
candidate `Vector2i` values from row/column indices. The source does not expose a
separate origin or transform in the snapshot, so runtime validation is required
before treating indices as world coordinates.

## Required evidence for changes

- Area hash and area id.
- Snapshot dimensions and data length.
- Coordinate origin and scale used by the planner.
- Path validation result for each selected target.
- Fallback mode when the terrain plan is unavailable.
