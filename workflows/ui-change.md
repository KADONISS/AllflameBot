# Workflow: UI Change

## Purpose

Safely change DPB UI discovery or input.

## Audience

Agents changing Element, coordinate or screenshot paths.

## Update when

Update when adapter order or UI fixture requirements change.

## Steps

1. Capture/inspect the visible UI and Element tree if available.
2. Prefer stable Element/native controls.
3. Scope coordinate fallback to the current control/window.
4. Release input in every path.
5. Verify a fresh postcondition.
6. Log selected mode and fallback reason.
7. Update compatibility docs and UI fixtures.
