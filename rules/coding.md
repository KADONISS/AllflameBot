# Coding Rules

## Purpose

Keep changes compatible with the existing C# 7.3, .NET Framework 4.8 plugin.

## Audience

Developers and AI coding agents.

## Update when

Update when conventions or supported language features change.

## Rules

- Prefer small methods with one observable responsibility.
- Preserve the existing naming style and structured log vocabulary.
- Do not introduce APIs newer than the project target without checking runtime
  availability.
- Avoid new global mutable state; if unavoidable, define reset and ownership.
- Do not silently change task order or phase guards.
- Keep comments focused on non-obvious invariants, not line-by-line narration.
