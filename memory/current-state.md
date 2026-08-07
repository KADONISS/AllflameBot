# Current State

## Updated

2026-08-08

## Repository

- Branch: `main`
- Current source release: `0.23.164`
- Target: .NET Framework 4.8, x64, C# 7.3
- Runtime shape: one DPB plugin assembly with ordered ObserverTasks
- Automated tests/CI: not currently present
- Notification delivery: bounded dashboard/immediate queues with HTTPS Discord
  webhook validation and schema-44 settings migration
- Runtime reliability: bounded interaction and movement-finish waits release
  process-hook input and queue fresh observation after a timeout; Bathysphere
  path reset releases input before and after navigation reload.

## Active architecture work

AI-native documentation and context structure has been added. Source remains a
large monolith; logical module pages describe boundaries without claiming a
source refactor.

## Update trigger

Update after each release, architecture migration or confirmed runtime discovery.
