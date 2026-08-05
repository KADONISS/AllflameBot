# Troubleshooting

## Purpose

Provide an evidence-first workflow for stalls, loops, wrong targets and UI
failures.

## Audience

Operators, reviewers and agents analyzing logs.

## Update when

Update when a new failure signature, recovery route or diagnostic artifact is
confirmed.

## Depends on

[`workflows/bug-investigation.md`](../workflows/bug-investigation.md),
[`docs/modules/reliability.md`](modules/reliability.md) and
[`knowledge/known-limitations.md`](../knowledge/known-limitations.md).

## Evidence to collect

- Log start/end timestamps and bot version.
- Area id/hash, phase, player position and active task.
- Last progress/watchdog signature and retry count.
- UI visibility and adapter mode.
- Movement owner, target, path result and navigation revision.
- Inventory/lantern/capsule/chest state relevant to the task.
- Any stop reason and whether the stop came from user or recovery code.

## Classification

1. Observation stale or contradictory.
2. Task guard never becomes true.
3. Action input not sent or held input not released.
4. Action sent but postcondition not observed.
5. Path target invalid, blocked or in the wrong coordinate space.
6. Retry loop, competing task ownership or terminal stop.

Do not fix a symptom by adding an unconditional stop. First identify whether a
fresh observation, route invalidation, UI reacquisition or state reset can
restore progress.
