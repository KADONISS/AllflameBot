# Architecture Rules

## Purpose

Protect runtime boundaries while the monolith is gradually documented or split.

## Audience

Agents performing feature work or refactors.

## Update when

Update when an accepted ADR changes a boundary.

## Rules

- Preserve the single-DLL DPB contract.
- Keep observation, decision, action and verification conceptually separate.
- Make state ownership explicit before adding a property.
- Route movement through the coordinator and mover contract.
- Route UI input through an adapter with a fallback order.
- Record architectural changes in an ADR when they affect dependencies or
  recovery semantics.
