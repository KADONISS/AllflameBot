# Changelog

All notable changes to AllflameBot are documented in this file.

## 0.23.130 - 2026-08-03

- Scan configured Message in a Bottle stash tabs once per bot session and
  preserve their logical counts across Hub, map and Voyage area transitions.
- Reconcile only the selected tab after a withdrawal, skip repeated stash
  checks when the cached combined supply is below the opening threshold, and
  invalidate the snapshot on bot restart, character change or Bottle setting
  changes.
- Defer Bottle processing while a Chart Device request or native Deepwater
  Chart Device window is active so it cannot close the loaded device before
  Descend and portal entry complete.
- Add session snapshot, cached supply, cache adjustment and Chart Device
  deferral markers for runtime verification.

## 0.23.129 - 2026-08-03

- Send valuable Message in a Bottle rewards through an immediate Discord
  webhook with `@everyone` and explicit allowed mentions when the configured
  value threshold and tag option are enabled.
- Avoid queuing a duplicate valuable-item dashboard alert for the same Bottle
  reward, while retaining the reward embed and session last-event update.
- Log Bottle delivery mode, mention eligibility and the actual mention flag
  used by successful Discord sends.

## 0.23.128 - 2026-08-03

- Cache nine stable Voyage Planner Chart slot coordinates from one snapshot
  and place them as a fast Ctrl-click batch with bounded intermediate/final
  convergence checks.
- Retry only missing Chart placements through the existing verified click
  path when the UI changes or the batch is only partially accepted.
- Add internal traversal, loot, chest, object and Capsule approach profiles;
  ordinary interactions transition later to faster precision clicks while
  Capsule keeps its conservative 130-unit single-click radius.
- Reuse converging paths briefly, release held movement before interaction,
  and log batch placement, approach phase and distance convergence markers.

## 0.23.127 - 2026-08-03

- Normalize and compare the DPB vendor Accept button text without case
  sensitivity so the runtime value `accept` is treated as ready.
- Keep `SellUi.TradeControl.Accept(true)` as the native action and continue
  confirming completion from Sell UI closure or an empty offer inventory.
- Log raw and normalized button text plus a dedicated confirmed-sale marker
  for diagnosing future localization or UI-state differences.

## 0.23.126 - 2026-08-03

- Prefer DPB's native `ResurrectToCheckPoint` API after the death UI grace
  period instead of immediately focusing the game and scanning the screen.
- Confirm respawn from the actual player state, area transition and resurrect
  panel state rather than treating a successful API return as completion.
- Retry the native API up to three times and retain the existing two-frame
  visual checkpoint detector only as a bounded fallback.
- Add explicit API state, attempt, rejection, confirmation and fallback log
  markers while preserving Chart/Voyage abandonment and post-death `/boat`.

## 0.23.125 - 2026-08-03

- Make Capsule extraction exclusive and mandatory as soon as a Voyage reaches
  `0/x`, cancelling chest grace, ground-loot work and further exploration.
- Prefer and force-lock any live targetable Capsule within 150 units over a
  stale reserved or registry-only anchor, even when it was not traversed.
- Remove the zero-lantern rescue deadline and maximum-cycle stop; exhausted
  strategies now rotate after a short cooldown while retaining the two-reload
  limit for each anchor.
- Keep rescue progress explicit: accepted movement input does not count as
  convergence toward the Capsule.

## 0.23.124 - 2026-08-03

- Resolve the opened Allflame Capsule through the stable
  `deepwater_extraction_window` IdLabel without hardcoding its GUI root index
  or runtime addresses.
- Resolve `Send to Surface` and `Call Bathysphere` from their exact extraction
  paths, with caption lookup restricted to that window when paths change.
- Use the stable extraction snapshot for Capsule UI context and both action
  buttons while retaining bounded geometry and visual detection as fallbacks.
- Re-resolve buttons immediately before clicking and log input separately from
  semantic acknowledgement of the surface transfer or Bathysphere call.

## 0.23.123 - 2026-08-03

- Convert Voyage Planner Chart indices through DPB's
  `InventorySlotUiElement.SlotClickLocation` so placement follows the current
  client resolution and UI scale instead of unreliable child-element centers.
- Reject Chart coordinates outside the game client and use the existing visual
  detector only when DPB cannot resolve a valid native grid coordinate.
- Track the real 60-slot inventory index, distinguish input submission from a
  confirmed transfer, and continue requiring inventory or board convergence
  before counting a Chart as placed.
- Synchronize the runtime bot version marker with the release metadata.

## 0.23.122 - 2026-08-03

- Resolve Voyage Planner from the stable `deepwater_voyage_window` IdLabel,
  including its 60-slot Chart pool, nine-slot board, Clear Board button and
  Begin Voyage button without requiring screen capture.
- Detect available and placed Charts from their per-slot `L:` labels, clear
  interrupted boards through the UI element, and Ctrl-click verified Chart
  slots until all nine board positions are filled.
- Verify each transfer from pool-count or board-count convergence, start the
  Voyage only after nine confirmed placements, and retain the existing visual
  implementation solely as a fallback when the stable UI cannot be resolved.

## 0.23.121 - 2026-08-03

- Resolve the Chart Device from the stable `deepwater_charts_window` IdLabel
  before bounded recursive UI scans, then click its visible Descend element
  before falling back to image recognition.
- Remove blind resolution-based Descend clicks and confirm the action only
  after the Chart Device UI closes; retry a still-open UI at most three times.
- Prevent the existing hub Bathysphere from being treated as proof that
  Descend succeeded, and bound portal appearance and entry with controlled
  recovery stops instead of an infinite interaction loop.

## 0.23.120 - 2026-08-03

- Deposit remaining pre-Voyage Charts into Chart Storage first, confirm each
  inventory transfer, and try all 15 storage pages before using a fallback.
- Treat the personal Chart stash tab as optional: blank, missing, unavailable
  or transfer-rejecting tabs no longer stop the active Voyage.
- Invalidate depleted Chart Storage page caches after a confirmed deposit so
  later withdrawal cycles can discover newly stored Charts.

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
