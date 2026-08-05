# Workflow: Architecture Change

## Purpose

Make structural changes traceable and reversible.

## Audience

Agents refactoring modules or dependencies.

## Update when

Update when ADR or migration requirements change.

## Steps

1. Update the knowledge graph and dependency map.
2. State current behavior and target boundary separately.
3. Write or update an ADR before broad refactoring.
4. Preserve host/package compatibility at every step.
5. Migrate one boundary at a time with characterization tests.
6. Update AGENTS, module context and workflow routes.
