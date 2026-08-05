# ADR 0002: Ordered ObserverTask Pipeline

- Status: Accepted (documented current contract)
- Date: 2026-08-06
- Depends on: `architecture/runtime-flow.md`

## Context

The bot registers an ordered list of observer tasks and runs them with an
until-handled behavior. Multiple tasks can match the same phase.

## Decision

Task order is the runtime arbitration mechanism. A task owns a cycle when it is
the first matching handler, and stores multi-cycle progress in shared state.

## Consequences

- Reordering tasks is a behavioral change.
- New tasks need explicit phase guards and priority analysis.
- Tests and logs must cover overlapping task conditions.
