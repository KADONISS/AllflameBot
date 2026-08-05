# AllflameBot Agent Guide

## Purpose

This file is the root operating contract for AI coding agents working on the
Allflame/Deepwater DreamPoeBot plugin.

## Audience

Read it before any code, documentation, test, build or release change.

## Required reading

1. [`.agents/index.md`](.agents/index.md)
2. [`architecture/overview.md`](architecture/overview.md)
3. [`docs/dpb-compatibility.md`](docs/dpb-compatibility.md)
4. The relevant module context under [`docs/modules/`](docs/modules/)
5. The relevant rule and workflow under [`rules/`](rules/) and [`workflows/`](workflows/)

## Project contract

- Target .NET Framework 4.8, x64 and C# 7.3.
- Produce one plugin assembly named `AllflameBot` unless a change explicitly
  updates the DreamPoeBot deployment contract.
- Preserve the root [`AllflameBot.csproj`](AllflameBot.csproj),
  [`3rdparty.json`](3rdparty.json) and `Data` asset contract.
- Treat DreamPoeBot assemblies as external dependencies. Do not replace their
  APIs with guessed abstractions or undocumented behavior.
- Keep runtime settings, logs, price caches, screenshots, binaries and secrets
  out of Git.

## Runtime rules

- The ordered `ObserverTask` pipeline is a priority system. Changing task order
  can change behavior even when the changed task is otherwise correct.
- Prefer refresh, retry, route invalidation, UI reacquisition and coroutine
  recovery over adding automatic stop behavior.
- Do not add a new self-stop path, broaden an existing stop condition, or turn a
  recoverable transient fault into a terminal stop without documenting the
  decision and its user-visible impact.
- Every UI or input action needs a postcondition or an explicit reason why a
  postcondition is unavailable.
- Do not let exploration override safety, capsule extraction, chest/loot debt or
  other higher-priority movement ownership.
- Never hide a coordinate transform, DPB version assumption or fallback change.
  Log it and document it.

## Source of truth

- Current behavior: source code and runtime logs.
- Build/deployment contract: `AllflameBot.csproj`, `3rdparty.json` and
  [`docs/dpb-compatibility.md`](docs/dpb-compatibility.md).
- Domain rules: [`knowledge/`](knowledge/) and accepted ADRs.
- Normative engineering behavior: [`rules/`](rules/).
- Historical behavior: [`CHANGELOG.md`](CHANGELOG.md), not a substitute for
  current source behavior.

## Change workflow

1. Route the task with [`.agents/context-routing.md`](.agents/context-routing.md).
2. Inspect the source and relevant evidence before proposing behavior.
3. State the affected task, state, adapter, external dependency and fallback.
4. Make the smallest coherent change.
5. Update tests, logs, architecture, knowledge and changelog as applicable.
6. Build with the matching DPB dependencies and report any unavailable check.
7. Review `git diff`, then commit with an action-oriented message.

## When to ask the human

Ask only when the task requires a new external authority, an irreversible
destructive action, a product decision not recoverable from the repository, or
credentials/access that cannot be discovered locally. Do not pause for routine
implementation choices that are covered by these documents.

## Update this file when

Update `AGENTS.md` when the build target, plugin contract, task ownership,
required reading, safety policy or release workflow changes.

## Dependencies

This file routes to `.agents`, `architecture`, `docs`, `rules`, `workflows` and
the relevant module context. It must not duplicate their detailed content.
