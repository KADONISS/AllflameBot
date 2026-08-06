# ADR 0003: Layered UI Fallbacks

- Status: Accepted (documented current contract)
- Date: 2026-08-06
- Depends on: `architecture/boundaries/ui-input.md`

## Context

DPB UI exposure can vary by client state. The plugin uses native semantic APIs,
Element paths and visual or coordinate fallbacks for several interactions.

## Decision

Use a validated native semantic action first when it is available and its
postcondition is known. Otherwise use validated Element controls, scoped
coordinate input and visual matching in that order. Confirm the resulting state
on a fresh observation cycle. Voyage Planner native actions remain opt-in until
shadow snapshots agree across consecutive cycles.

## Consequences

- Each fallback must identify its mode in logs.
- Screenshot detection cannot by itself prove game acceptance.
- UI changes need fixtures or runtime captures when possible.
