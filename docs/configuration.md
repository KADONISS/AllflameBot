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
- Native DPB adapters and Voyage Planner canary behavior.
- Pricing and unique artwork resolution.
- Discord notifications.

## Discord notification settings

Notification settings are migrated with `SettingsSchemaVersion` and remain
runtime user data. The webhook is accepted only when it is an HTTPS Discord
webhook URL; it is never written to structured logs.

- `DiscordNotificationsEnabled` controls outbound delivery.
- `SendPeriodicSummary` and `NotificationIntervalMinutes` control periodic
  updates to the single live dashboard message. Boundary updates remain
  controlled by `NotifyMapStarted`, `NotifyMapCompleted` and the throttled
  `NotifySulphurCollected` setting.
- Chart/Voyage supply alerts use `NotifyLowChartSupply`,
  `NotifyEmptyChartSupply`, `LowChartWarningThreshold`,
  `NotifyLowVoyageStash` and `VoyageStashEveryoneThreshold`.
- Loot/reward alerts use `NotifyValuableItems`, fractional
  `ValuableItemMinDivines`, `TagEveryoneForValuableItems` and
  `NotifySecretBottleRewards`.
- Runtime alerts use `NotifyPriceWarnings`, `NotifyDeaths`,
  `NotifyRecoveryFailures` and `NotifyTerminalStop`.
- Delivery runs outside the main task cycle through a bounded queue. Discord
  failures are retried and logged without changing movement, recovery or stop
  ownership.

The dashboard content is Allflame-specific: it reports Chart/Voyage progress,
Lantern, Sulphur, Chart inventory, Chart Storage, Voyage Stash, confirmed loot,
profit, current activity and the latest alert.

## Caution

A setting visible in the GUI is not automatically a runtime guarantee. Before
documenting or relying on a property, confirm a source consumer and a log or
postcondition. In particular, audit settings whose names imply terrain planning,
unlit-area avoidance or repeated-failure stopping.

Native low-risk feature flags default to enabled after settings migration. They
cover Sovereign travel, Capsule actions, Loot UI reads, Bottle reveal,
Chart Device reads and Deepwater counters. Native Voyage Planner reads run as a
shadow comparison; native Planner actions are opt-in and remain behind their
postcondition/canary gate. Voyage Stash stays on the stable stash adapter.
