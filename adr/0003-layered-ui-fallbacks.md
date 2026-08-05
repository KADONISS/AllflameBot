# ADR 0003: Layered UI Fallbacks

- Status: Accepted (documented current contract)
- Date: 2026-08-06
- Depends on: `architecture/boundaries/ui-input.md`

## Context

DPB UI exposure can vary by client state. The plugin uses Element/native paths
and visual or coordinate fallbacks for several interactions.

## Decision

Use validated Element/native controls first, scoped coordinate input second and
visual matching last. Confirm the resulting state on a fresh observation cycle.

## Consequences

- Each fallback must identify its mode in logs.
- Screenshot detection cannot by itself prove game acceptance.
- UI changes need fixtures or runtime captures when possible.
