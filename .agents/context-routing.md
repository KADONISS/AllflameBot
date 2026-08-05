# Context Routing

## Purpose

Provide repeatable required reading for common changes.

## Audience

Agents planning or reviewing a change.

## Routing rules

- Start from the affected runtime phase, not from a filename alone.
- Read the module context and its direct dependencies before editing.
- For a task-order change, read the runtime flow and state model.
- For a coordinate or planner change, read map-model knowledge and navigation rules.
- For a UI change, read the DPB compatibility contract and UI interaction rules.
- For a stop/recovery change, read reliability context and the error-handling rule.
- For a release change, read compatibility, release docs and release workflow.

## Impact questions

Before editing, answer:

1. Which `ObserverTask` can own this cycle?
2. Which state object is read and which component owns its mutation?
3. Which DPB API or filesystem/network boundary is touched?
4. What is the primary path and what are the fallbacks?
5. What postcondition or log evidence proves success?
6. Could the change create a new loop, stop path or movement ownership conflict?

## Update this file when

Update it when task routing or required-reading relationships change.

## Dependencies

Depends on `AGENTS.md`, `architecture/overview.md`, `architecture/state-model.md`
and the module documents under `docs/modules/`.
