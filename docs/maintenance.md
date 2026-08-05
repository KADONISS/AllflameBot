# Documentation Maintenance

## Purpose

Define how documentation stays synchronized with future code changes.

## Audience

Developers and AI agents completing a change.

## Update when

Update when the documentation ownership matrix or release workflow changes.

## Change matrix

| Code change | Update |
| --- | --- |
| New task or task-order change | Runtime flow, module AGENT, ADR if priority changes |
| State ownership/property change | State model, module context, tests |
| Business behavior change | Business rule, knowledge, changelog |
| DPB/API/reference change | Compatibility docs, dependency map, release notes |
| Planner/coordinate change | Map model, navigation rule, planner fixture |
| UI/fallback change | UI boundary, compatibility docs, UI fixture |
| Recovery/stop change | Reliability context, recovery rule, ADR and changelog |
| Setting change | Configuration docs, GUI/module context, migration notes |
| Release process change | Release docs, workflow, checklist and AGENTS |

## Promotion rule

Temporary observations begin in `memory/`. Once confirmed, promote them to
`knowledge/`, `docs/`, `architecture/` or an ADR and remove the duplicate memory
entry.

## Review rule

Every source change should answer whether documentation, tests, fixtures,
changelog or release metadata changed. A deliberate no-change answer is valid
when recorded in the handoff.
