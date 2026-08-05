# Workflow: Bug Investigation

## Purpose

Turn a log or runtime symptom into an evidence-backed diagnosis.

## Audience

Agents and developers debugging stalls, loops, wrong targets or stops.

## Update when

Update when a new diagnostic artifact or repeatable classification is added.

## Steps

1. Preserve the original log and identify the bot version.
2. Extract phase, area hash, active task, position, target, mode and watchdog.
3. Reconstruct task arbitration and state transitions.
4. Search source for all retry, recovery and stop paths.
5. Reproduce with a fixture or live evidence when possible.
6. Fix state/strategy/input first; avoid adding automatic stops.
7. Add regression evidence and update known limitations.
