# Current State

## Updated

2026-08-12

## Repository

- Branch: `main`
- Current source release: `0.23.168`
- Target: .NET Framework 4.8, x64, C# 7.3
- Runtime shape: one DPB plugin assembly with ordered ObserverTasks
- Automated tests/CI: not currently present
- Notification delivery: bounded dashboard/immediate queues with HTTPS Discord
  webhook validation and schema-44 settings migration
- Runtime reliability: DPB interaction actions use one coroutine task per tick;
  native/task-specific timeouts release process-hook input and queue fresh
  observation; Bathysphere path reset releases input before and after reload;
  native blocking-window cleanup runs before Sovereign travel and boat staging
  movement with next-cycle verification; successful Chart Device Descend enters
  a post-Descend transition state without polling native Chart Device UI, using
  area/capsule postconditions and bounded extraction recovery.

## Active architecture work

AI-native documentation and context structure has been added. Source remains a
large monolith; logical module pages describe boundaries without claiming a
source refactor.

## Update trigger

Update after each release, architecture migration or confirmed runtime discovery.
