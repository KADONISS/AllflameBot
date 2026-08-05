# Test Strategy

## Purpose

Define the future test layout without pretending that live DPB tests already
exist.

## Audience

Agents adding regression coverage and reviewers assessing validation.

## Update when

Update when a test project, fixture harness or CI pipeline is added.

## Planned layers

- `Unit`: pure state, planner, coordinate, filter and normalization behavior.
- `Contract`: DPB adapter behavior against matching external assemblies.
- `Fixtures`: sanitized logs, UI trees, terrain snapshots and inventory deltas.
- `Live`: operator-run client checks documented separately from deterministic
  tests.

## Current status

No test project is currently present. Do not report the repository as automated-
test validated until a test command has actually run.
