# Lessons Learned

## Purpose

Capture evidence-backed lessons from previous maintenance work.

## Audience

Agents investigating regressions or planning similar changes.

## Update when

Update after a confirmed incident, release regression or runtime discovery.

## Current lessons

- Verify every UI action with a fresh snapshot; coordinate correctness alone is
  insufficient.
- Do not treat the Voyage Planner board as world terrain.
- Preserve high-priority capsule and extraction ownership while improving
  exploration.
- Structured logs should identify area hash, task, mode, target, input and
  postcondition.
- A changelog claim is not proof that the current source no longer contains an
  old stop path; search the source.
- Runtime recovery and terminal stop must be documented separately.
- Do not resolve the Chart Storage Element tree on the ordinary boat scene;
  wait for a fresh visible-UI observation after the interaction is issued.
