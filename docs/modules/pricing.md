# Pricing Module Context

## Purpose

Context for price cache, poe.ninja updates, fixed prices and unique artwork
resolution.

## Audience

Agents changing item valuation or notification calculations.

## Current source

[`AllflamePrices.cs`](../../AllflamePrices.cs),
[`AllflameUniqueResolver.cs`](../../AllflameUniqueResolver.cs),
[`AllflameNotifications.cs`](../../AllflameNotifications.cs) and
[`Data/uniqueArtMapping.default.json`](../../Data/uniqueArtMapping.default.json).

## Owns

- Local price cache and backups.
- Remote price update and normalization.
- Fixed-price overlay.
- Artwork-based unique resolution.

Notification calculations consume only confirmed pickup quantities and current
price quotes. Valuable-loot alerts use the configured fractional Divine
threshold; stale or missing prices produce an optional warning and are not
treated as proof of zero value.

## Required checks

Treat unavailable or stale prices as uncertainty, not zero-value proof. Preserve
cache schema and do not commit personal price files or credentials.
