# UI Interaction Rules

## Purpose

Make UI automation deterministic, scoped and recoverable.

## Audience

Agents changing Element, coordinate or screenshot interactions.

## Update when

Update when a UI adapter or fallback order changes.

## Rules

- Prefer a validated Element or native control over screen-wide text matching.
- Scope coordinate clicks to the current window/control geometry.
- Capture a fresh snapshot before and after a consequential action.
- Verify the intended state transition, not only that input was sent.
- Release key/button state in `finally`-equivalent cleanup.
- Log `Mode`, `InputSent`, `Postcondition` and fallback reason.
