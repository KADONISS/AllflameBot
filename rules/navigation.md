# Navigation Rules

## Purpose

Prevent wrong-coordinate routing, competing movement and exploration loops.

## Audience

Agents changing movers, terrain snapshots or planners.

## Update when

Update when coordinate transforms, planner priority or movement ownership changes.

## Rules

- Include area hash with every cached coordinate plan.
- Make origin, scale, layer and coordinate-space assumptions explicit.
- Validate a selected target with the current pathfinder.
- Invalidate blocked targets and advance strategy instead of retrying forever.
- Route all movement through `AllflameMovementCoordinator`.
- Preserve capsule, extraction, safety and loot priorities over frontier targets.
