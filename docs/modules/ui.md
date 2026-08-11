# UI Module Context

## Purpose

Context for DPB Element discovery, native UI APIs, coordinate input and visual
fallbacks.

## Audience

Agents changing any UI interaction.

## Current source

UI adapters are in [`AllflameBot.cs`](../../AllflameBot.cs); WPF settings UI is in
[`AllflameBotGui.cs`](../../AllflameBotGui.cs).

## Owns

- UI discovery and stable element resolution.
- Native DPB semantic controls for process-covered actions.
- Input dispatch and held-input cleanup.
- Postcondition snapshot and fallback mode logging.
- Post-Descend UI probe quarantine: area/capsule state owns the transition
  postcondition until bounded recovery explicitly reacquires the Chart Device.
- Notification settings UI, masked webhook entry and delivery-status display.

## Required checks

Use a validated native semantic action when available, then the narrowest valid
Element control; avoid global text clicks, verify the next state, and retain
visual fallback only as a final path.
Context-level raw Element traversal is skipped entirely in
`DeepwaterEncounter` and `DeepwaterEncounterVoyage`. Native semantic reads,
object snapshots and action-scoped Element/visual fallbacks own those states;
the remaining global scan is bounded by node/time budgets and disabled for the
current `AreaHash` after a slow or incomplete read.
