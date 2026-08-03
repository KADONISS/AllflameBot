# Changelog

All notable changes to AllflameBot are documented in this file.

## 0.23.119 - 2026-08-03

- Accept comma-separated Message in a Bottle stash tabs, scan every resolved
  personal or guild tab, and remember available, depleted and failed tabs for
  the active Bottle cycle.
- Combine inventory and all configured stash counts before applying the open
  threshold, then withdraw across tabs up to the per-session Bottle limit.
- Complete one bounded Bottle cycle before yielding to map preparation, while
  continuing to reserve unopened Bottles from the normal loot stash task.

## 0.23.118 - 2026-08-03

- Defer Chart supply checks while a requested Chart Device cycle owns the
  current Chart, preventing the last inventory Chart from being mistaken for
  exhausted supply immediately after it is loaded into the device.
- Confirm and log native Chart Device contents before Descend, and distinguish
  a loaded device from genuinely exhausted Chart Storage supply.

## 0.23.117 - 2026-08-03

- Restrict the Secret Bottle value threshold and NPC selling to confirmed
  Unique rewards; currency, gems, scarabs and all other rarities are kept.
- Preserve transferred reward state, wait for DPB's three-second vendor Accept
  lock to clear, and confirm the offer was consumed before counting a sale.
- Add Bottle reward notifications after inventory placement is confirmed, with
  independent Valuable Items alerts for rewards above the configured threshold.
- Add a Bottle Rewards card to the Notifications tab and migrate existing
  settings with notifications enabled by default.

## 0.23.116 - 2026-08-03

- Remember the confirmed remaining Chart count for every main/sub storage page
  throughout the bot session and skip pages already depleted to zero.
- Skip an entire main tab when all five of its pages are depleted, and avoid
  reopening Chart Storage after all 15 cached pages have been exhausted.
- Preserve unknown pages after UI failures, update cached counts only from
  stable reads or confirmed transfers, and reset the cache only with the bot
  session.

## 0.23.115 - 2026-08-03

- Stop raw Chart Storage withdrawal immediately when the configured inventory
  Chart target is reached instead of emptying every remaining storage page.

## 0.23.114 - 2026-08-03

- Read Chart Storage page dictionaries directly without comparing item screen
  coordinates to transformed parent bounds, fixing item loss on main tabs 2
  and 3.
- Confirm the active main tab through its enabled state and the active numbered
  tab through the button moved to child 5, as established by all 15 UI dumps.
- Keep the confirmed page path `2/0/1/{main}/0/1/{sub}/0`, add dictionary and
  selection diagnostics, and deduplicate repeated top-level storage roots.
- Hand partial Chart Storage supply to Voyage Stash for target top-up while
  preserving the existing no-top-up behavior for an already active batch.

## 0.23.113 - 2026-08-03

- Treat every visible item in the confirmed Chart Storage page grid as a
  Chart and withdraw it directly without native inventory metadata mapping.
- Resolve every requested main/numbered page from its exact UI path instead
  of reusing a cached layer from a previously selected page.
- Wait for a key-only raw-grid signature to stabilize before accepting page
  contents, including a delayed confirmation path for genuinely empty pages.
- Re-resolve after each Ctrl-click and require the raw key or item count to
  disappear before continuing, with explicit inventory-full and click-failure
  diagnostics.

## 0.23.112 - 2026-08-03

- Confirm a Chart Storage page from its active tab and one exact page-layer
  resolution instead of requiring two resolver reads inside a short deadline.
- Preserve the last valid storage UI handles across intermittent resolver
  misses and accept empty Chart pages as valid scan results.
- Revalidate the visible main and numbered tabs before each page, and log page
  activity, layer resolution, signatures, and resolver attempts separately.

## 0.23.111 - 2026-08-03

- Start the 3x5 Chart Storage UI scan as soon as the confirmed page layer is
  available instead of reopening the window when native binding is delayed.
- Probe requested inventory 151 first, then dynamically locate and cache a
  1x60 all-Chart inventory by requested ID and exact page LocalId matches.
- Keep raw page discovery read-only until native mapping succeeds; withdrawal
  now uses FastMove only and safely falls back to Voyage Stash on bind timeout.
- Report UI, raw-page, native-candidate, mapping, skipped, and withdrawal
  diagnostics separately for live verification.

## 0.23.110 - 2026-08-03

- Resolve Chart Storage from its confirmed UI tree instead of interpreting
  arbitrary element geometry as a 12x5 inventory grid.
- Visit the three main categories and five numbered pages in forward order,
  and confirm each page by visibility before reading or withdrawing Charts.
- Bind requested server inventory 151 and require exact UI dictionary-key to
  native LocalId matches, structurally excluding the 4x9 Sulphur area.
- Add per-page, matched, skipped, and withdrawn Chart diagnostics while
  preserving Voyage Stash fallback behavior.

## 0.23.109 - 2026-08-03

- Use single-click movement near interaction targets instead of leaving
  `BeginUseAt` held, so the player stops inside the Allflame Capsule ring.
- Keep normal exploration and covered-route travel on the original held-input
  mover; precision clicks are scoped to objects, Capsules, and chest approach.
- Clear movement input before and after DPB action synchronization and accept
  the full configured Capsule interaction range.

## 0.23.108 - 2026-08-03

- Bound zero-lantern Capsule rescue to three cycles and a 60-second deadline.
- Treat registry-only Capsule anchors as bounded live-object reacquisition targets.
- Prevent interaction stages from running without a valid live Capsule object.
- Track explicit convergence, live-target acquisition, UI progress, and route deltas.
- Stop with `CapsuleRescueExhausted` and `RecoveryStop` after bounded recovery fails.
- Establish the standalone AllflameBot source repository baseline.
