# Voyage Debugging Skill

## Use when

Analyzing Voyage stalls, boat/Bathysphere waits, capsule priority or lantern
coverage.

## Read first

`docs/modules/voyage.md`, `docs/modules/reliability.md`,
`knowledge/business-rules/voyage.md` and `docs/troubleshooting.md`.

## Method

Extract phase, area hash, player position, active task, movement owner, capsule
state, lantern state, Bathysphere state, watchdog and stop reason. Reconstruct
the first-handler-wins sequence before proposing a change. Prefer reacquisition,
state correction or route invalidation over self-stop.

## Output

State evidence, root cause, affected ownership, fix options, validation and
residual risk.
