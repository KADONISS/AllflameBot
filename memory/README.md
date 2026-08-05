# Project Memory

## Purpose

Store short-lived operational context that is useful between maintenance
sessions but should not replace source, architecture or ADRs.

## Audience

Agents continuing work after a context boundary.

## Update when

Update after an incident, release, compatibility discovery or completed
migration. Add dates and evidence.

## Rules

- Do not store secrets or personal runtime paths.
- Mark uncertain information as uncertain.
- Promote stable knowledge to `knowledge/`, `docs/` or an ADR.
- Remove stale notes instead of letting memory become a second changelog.

## Files

- [`current-state.md`](current-state.md)
- [`known-failures.md`](known-failures.md)
- [`compatibility-history.md`](compatibility-history.md)
- [`lessons-learned.md`](lessons-learned.md)
