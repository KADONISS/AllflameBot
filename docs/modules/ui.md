# UI Module Context

## Purpose

Context for DPB Element discovery, native UI APIs, coordinate input and visual
fallbacks.

## Audience

Agents changing any UI interaction.

## Current source

UI adapters are in [`AllflameBot.cs`](../../AllflameBot.cs) and
[`BackgroundUiInput.cs`](../../BackgroundUiInput.cs); WPF settings UI is in
[`AllflameBotGui.cs`](../../AllflameBotGui.cs).

## Owns

- UI discovery and stable element resolution.
- Input dispatch and held-input cleanup.
- Postcondition snapshot and fallback mode logging.

## Coordinate and input contract

DPB `Element.CenterClickLocation()` and inventory slot helpers provide screen
coordinates. Visual matchers operate in client coordinates. The input adapter
keeps these paths explicit: `ClickScreen` converts through the current client
window stamp, while `ClickClient` posts the supplied client point directly.
When the game is covered, input is sent to its HWND with `PostMessage`; the
adapter never moves the physical cursor in that mode.

`PrintWindow` is used as a best-effort covered-window capture. A queued input
event is not a successful state transition, so callers must wait for and verify
the resulting UI or inventory change.

## Required checks

Use the narrowest valid control scope, avoid global text clicks when an element
exists, verify the next state, and retain visual fallback only as a final path.
