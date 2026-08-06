# Map Model

## Purpose

Describe the distinction between Chart terrain, Voyage terrain and the Voyage
Planner board.

## Audience

Agents changing map acquisition or exploration.

## Update when

Update when DPB terrain APIs, coordinate transforms or planner ownership change.

## Current facts

- `LokiPoe.TerrainData.Cache` exposes cached terrain buffers used by
  `AreaMapSnapshotService`.
- `AreaMapSnapshot` stores area identity, rows, columns, nav-cell size, data,
  fly data, jump mask and a walkability bit array.
- `GridExplorer`-style explored state is not the same as the full terrain grid.
- `DeepwaterVoyageUi.VoyageMap` is the Voyage Planner board, not world terrain.
- Voyage terrain uses the same DPB terrain/pathfinding family as Chart terrain,
  but its planner currently remains a rolling frontier implementation.
- Native Voyage Planner board fields are shadow diagnostics; stable Element slot
  occupancy remains authoritative until consecutive snapshots agree.

## Coordinate warning

The current Chart terrain planner creates `Vector2i` candidates from row/column
indices. The snapshot does not model a separate world origin or transform.
Runtime verification is required before claiming that every terrain cell maps
directly to a world coordinate.

## Required log fields

Area hash, area id, rows, columns, data length, walkable count, player position,
selected target, path result, planner mode and fallback mode.
