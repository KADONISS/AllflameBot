# Current State

## Updated

2026-08-07

## Repository

- Branch: `main`
- Current source release: `0.23.163`
- Target: .NET Framework 4.8, x64, C# 7.3
- Runtime shape: one DPB plugin assembly with ordered ObserverTasks
- Automated tests/CI: not currently present
- Notification delivery: bounded dashboard/immediate queues with HTTPS Discord
  webhook validation and schema-44 settings migration

## Active architecture work

AI-native documentation and context structure has been added. Source remains a
large monolith; logical module pages describe boundaries without claiming a
source refactor.

## Update trigger

Update after each release, architecture migration or confirmed runtime discovery.
