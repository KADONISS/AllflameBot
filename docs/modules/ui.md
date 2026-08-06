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
- Input dispatch and held-input cleanup.
- Postcondition snapshot and fallback mode logging.

## Required checks

Use the narrowest valid control scope, avoid global text clicks when an element
exists, verify the next state, and retain visual fallback only as a final path.
