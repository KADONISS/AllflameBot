# Performance Rules

## Purpose

Keep the main task cycle responsive and avoid expensive repeated scans.

## Audience

Agents changing observation, planners, UI scans or notifications.

## Update when

Update when timing budgets or background-worker behavior changes.

## Rules

- Keep the main cycle bounded and yield after work.
- Reuse stable snapshots only when area hash and freshness are valid.
- Throttle diagnostic logs without hiding state transitions.
- Do not perform network or large file work synchronously in a task cycle.
- Measure before optimizing planner or UI traversal.
- Preserve watchdog progress signals while reducing work.
