# Logging Rules

## Purpose

Make runtime evidence searchable and actionable.

## Audience

Agents adding or changing diagnostics.

## Update when

Update when the event vocabulary or log sink changes.

## Rules

- Use stable uppercase event names and pipe-delimited fields.
- Include task, phase, area hash and action mode at state boundaries.
- Log target, input result and postcondition separately.
- Log fallback selection and recovery reason.
- Throttle heartbeats, not transitions or failures.
- Sanitize values that can contain `|`, secrets or unbounded user text.
