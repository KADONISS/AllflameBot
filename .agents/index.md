# AI Context Index

## Purpose

Route an AI agent to the smallest reliable context set before it edits the
repository.

## Audience

All AI coding agents and human maintainers starting a task.

## Required first read

Read the root [`AGENTS.md`](../AGENTS.md), then choose the route below.

## Task routes

| Task | Required context |
| --- | --- |
| General code change | `architecture/overview.md`, `rules/coding.md`, relevant module context |
| Chart behavior | `docs/modules/chart.md`, `knowledge/business-rules/chart.md`, `rules/navigation.md` |
| Voyage behavior | `docs/modules/voyage.md`, `knowledge/business-rules/voyage.md`, `rules/navigation.md` |
| UI or input issue | `docs/modules/ui.md`, `rules/ui-interaction.md`, `docs/dpb-compatibility.md` |
| Stuck or recovery issue | `docs/modules/reliability.md`, `rules/error-handling.md`, `workflows/bug-investigation.md` |
| Pricing or unique artwork | `docs/modules/pricing.md`, `docs/configuration.md` |
| Tests | `rules/testing.md`, `tests/README.md`, relevant fixtures README |
| Documentation update | `rules/documentation.md`, `workflows/documentation-update.md` |
| Release or DLL packaging | `docs/release.md`, `workflows/release.md`, `rules/release.md` |

## Context order

1. Contract and constraints.
2. Current architecture and module context.
3. Relevant business rules and ADRs.
4. Source and tests.
5. Logs, screenshots or external evidence.

Do not treat an old changelog entry, memory note or prompt as proof of current
runtime behavior without checking source.

## Update this file when

Update it when a new module, task family, workflow, external dependency or
high-risk troubleshooting route is added.

## Dependencies

This index depends on the root `AGENTS.md` and the documents listed in its task
routes. It is a router, not a second architecture document.
