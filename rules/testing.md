# Testing Rules

## Purpose

Define honest validation when live DPB tests are limited.

## Audience

Agents adding or reviewing tests.

## Update when

Update when a test harness or CI pipeline is added.

## Rules

- Test pure state transitions, planners, normalization and policy without DPB.
- Use fixtures for logs, UI trees, terrain snapshots and inventory deltas.
- Use contract tests for DPB adapters when the matching binary is available.
- Treat live-client validation as integration evidence, not a substitute for
  deterministic tests.
- Never report a skipped or unavailable test as passing.
- Include a regression case for every fixed loop, coordinate or postcondition
  bug when feasible.
