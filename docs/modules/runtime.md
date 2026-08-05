# Runtime Module Context

## Purpose

Context for host lifecycle, task registration and main-loop changes.

## Audience

Agents changing `AllflameBot`, `ObserverTask` or lifecycle behavior.

## Current source

[`AllflameBot.cs`](../../AllflameBot.cs), especially the bot entry point, task
registration and main loop.

## Owns

- DPB callback contract.
- Ordered task pipeline.
- Coroutine startup, resume and rebuild.
- Bot start/stop lifecycle.

## Must not own

Feature-specific policy that belongs in a task or domain state store.

## Required checks

Check task order, phase guards, reset behavior, coroutine health and whether a
new exception path would stop or recover the bot.
