# Configuration

## Purpose

Describe configuration ownership and the main setting groups.

## Audience

Operators and agents changing `AllflameSettings` or the WPF settings UI.

## Update when

Update when a setting is added, renamed, migrated, removed or changes runtime
semantics.

## Depends on

[`AllflameSettings.cs`](../AllflameSettings.cs),
[`AllflameBotGui.cs`](../AllflameBotGui.cs),
[`rules/documentation.md`](../rules/documentation.md) and
[`docs/runtime-lifecycle.md`](runtime-lifecycle.md).

## Storage

The preferred settings file is `Documents/AllflameBot/AllflameBot.json`, with a
legacy fallback under the DPB/application settings path. Settings are runtime
user data and must not be committed.

## Groups

- General and chart execution.
- Voyage planning and voyage runner.
- Chest selection and priorities.
- Loot filters and ground pickup.
- Stash and transfer behavior.
- Bathysphere, lantern and extraction behavior.
- Secret Bottle processing.
- Pricing and unique artwork resolution.
- Discord notifications.

## Caution

A setting visible in the GUI is not automatically a runtime guarantee. Before
documenting or relying on a property, confirm a source consumer and a log or
postcondition. In particular, audit settings whose names imply terrain planning,
unlit-area avoidance or repeated-failure stopping.
