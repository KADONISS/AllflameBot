# ADR 0009: Allflame Notification Delivery Boundary

## Status

Accepted

## Context

Allflame notifications need to describe Chart Runner and Voyage Runner state,
but Discord delivery is an external and unreliable network boundary. The main
`ObserverTask` cycle must remain responsive and notification failures must not
change movement ownership, recovery decisions or terminal-stop policy.

## Decision

- `AllflameSession` and runtime state provide confirmed ledger/snapshot data;
  they do not perform network work.
- `AllflameNotifications` renders one coalesced live dashboard message and a
  bounded queue of immediate alerts.
- Dashboard updates use POST with `wait=true`, then PATCH by message ID, with
  404 recreation and bounded retry handling for transient failures and 429.
- Immediate alerts are delivered by a single background worker. Queue overflow,
  invalid webhook configuration and network failure are logged and dropped
  without stopping the bot.
- Webhooks must use HTTPS and a Discord webhook host/path. Secrets are not
  included in status text or structured logs.
- Notification settings are migrated with the settings schema and every
  visible setting must have an active runtime consumer.

## Consequences

The dashboard can lag briefly during network failure, but bot movement and
recovery remain independent. Operators get explicit delivery state in the UI.
The single dashboard message reduces Discord spam, while confirmed loot,
Bottle rewards, supply failures and terminal events remain visible as alerts.
