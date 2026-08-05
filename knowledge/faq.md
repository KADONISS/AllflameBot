# Frequently Asked Questions

## Purpose

Answer recurring repository questions using current source and architecture docs.

## Audience

Operators, developers and AI agents.

## Update when

Update when a question recurs or its answer changes.

## Is the Voyage Planner board the Voyage terrain map?

No. The planner board is a UI model of chart slots and modifiers. Voyage world
terrain comes from DPB terrain/pathfinding APIs.

## Does a sent click prove chart placement?

No. A fresh board/inventory postcondition is required.

## Does a recovery log prove the bot cannot stop?

No. Recovery and terminal stop are separate paths and must both be audited.

## Can the repository build without DPB binaries?

Not reliably. The project references external DPB and Newtonsoft.Json assemblies.

## Where should temporary runtime findings go?

Use `memory/` first, then promote confirmed facts to the appropriate stable
documentation.
