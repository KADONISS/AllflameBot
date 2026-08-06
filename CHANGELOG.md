# Changelog

All notable changes to AllflameBot are documented in this file.

## Unreleased

- Add an AI-native repository operating system with root and module context,
  architecture boundaries, business knowledge, rules, workflows, prompts,
  ADRs and release/test checklists.
- Document current DPB compatibility assumptions, map-coordinate uncertainty,
  task ownership and the distinction between recoverable faults and terminal
  stop paths.
- Remove obsolete release-history sections from `README.md`; release history now
  remains centralized in this changelog.

## 0.23.153 - 2026-08-06

- Add a native `DeepwaterVoyageSnapshot` backed by DPB's Voyage Planner API for
  planner visibility, available Charts, board occupancy and Begin state.
- Defer Chart Storage while the Voyage Planner is open, using the native state
  first and the stable Element root as a fallback.
- Stop using the visible-text Chart Storage summary as UI proof; resolve only
  from a fresh Chart Storage root and active page layer, ignoring stale cached
  controls.

## 0.23.152 - 2026-08-06

- Keep Chart Storage Element resolution out of the ordinary boat scene until a
  fresh UI snapshot confirms that the window is visible.
- Close blocking windows before the storage interaction, track the interaction
  cooperatively with an eight-second timeout, and confirm the opened page from
  a later UI snapshot and page-layer resolution.
- Bound storage open/resolve recovery to three attempts, refresh and reacquire
  the UI after a timeout, then continue through the existing Voyage Stash
  fallback without adding a bot stop.
- Add stage, postcondition, timeout and recovery diagnostics for Chart Storage.

## 0.23.151 - 2026-08-05

- Confirm the Call Bathysphere unused-Capsule warning through DPB Element
  nodes, with dialog-relative coordinates and scoped `OK` text fallbacks.
- Retain the existing screenshot matcher as the final warning confirmation
  fallback for clients whose warning modal is not exposed by the Element tree.
- Log the confirmation mode in both direct and visual Call Bathysphere paths.

## 0.23.150 - 2026-08-05

- Add an immutable `AreaMapSnapshot` backed by DPB terrain, walkability,
  fly-data and jump-mask buffers, stabilized per AreaHash.
- Add Chart terrain-guided frontier targets while retaining topology, Capsule,
  Lantern, chest and hybrid geometric planner ownership priorities.
- Add Chart terrain readiness, heartbeat, no-path and fallback diagnostics;
  terrain/API failures continue through the existing planner without stopping.

## 0.23.149 - 2026-08-05

- Recover Chart Device and Bathysphere transition failures by refreshing UI
  state and reacquiring the chart instead of stopping the bot.
- Add transition heartbeats with area, UI, chart and recovery state diagnostics
  to expose stalled Descend transactions.
- Keep coroutine disposal and progress watchdog failures recoverable so Voyage
  can continue without automatic stop behavior.

## 0.23.148 - 2026-08-05

- Add a Voyage terrain snapshot service backed by DPB terrain metadata and
  walkability data, stabilized per AreaHash before use.
- Validate geometric Voyage frontier targets against the live DPB pathfinder
  when terrain data is ready, while retaining the existing planner fallback.
- Add terrain readiness, dimensions, walkable-cell and no-path diagnostics;
  terrain failures retry softly without stopping the bot.

## 0.23.147 - 2026-08-05

- Make emergency Allflame Capsule routing select the Capsule itself instead
  of only steering the geometric frontier toward it.
- Preserve `CapsuleExtraction` movement priority through approach, reach and
  soft-defer recovery so topology/frontier movement cannot take ownership.
- Keep Capsule failures recoverable without adding automatic stop behavior.

## 0.23.146 - 2026-08-05

- Remove the incorrect absolute `+/-1500` world-coordinate limit that rejected
  valid Voyage positions and caused movement recovery loops.
- Record submitted movement intents and throttled mover results for diagnosing
  real pathfinder failures without adding automatic stop behavior.

## 0.23.145 - 2026-08-05

- Arbitrate movement ownership so safety, extraction and loot actions cannot
  be overridden by lower-priority Voyage exploration movement.
- Recover from stale navigation targets with revision invalidation, frontier
  no-progress detection and repeated-target fingerprint handling.
- Rebuild recoverable task and coroutine runtime faults in place, including
  interaction and DPB/UI exceptions, so Voyage can continue without adding
  automatic stop behavior.

## 0.23.144 - 2026-08-05

- Remove the new DPB Voyage API from Plan Board clearing, Chart placement and
  Begin Voyage flow.
- Restore the Element UI planner with the bounded visual Chart coordinate
  fallback used by the successful 9/9 placement path.
- Keep placement verification based on fresh UI state so stale coordinates do
  not repeatedly click the same Chart.

## 0.23.143 - 2026-08-05

- Fix Voyage Planner board occupancy to use the stable UI's actual Chart
  count instead of interpreting map directions and border data as occupied
  squares.
- Resolve inventory Chart click coordinates from the live stable slot
  elements, avoiding the invalid `6x10` grid-size assumption.
- Require a real stable board delta before confirming `FastMove()` placement,
  log postcondition mismatches, and use a bounded stable-element Begin Voyage
  fallback when the native button state is stale.

## 0.23.142 - 2026-08-04

- Use DreamPoeBot 0.3.29.23 `DeepwaterVoyageUi` as the primary Voyage
  Planner interface for open-state detection, Chart inventory reads, board
  state, Clear Board and Begin Voyage actions.
- Transfer Charts through the dedicated `VoyageInventoryControlWrapper`
  `FastMove()` API in bounded batches of three, then verify inventory or board
  deltas before issuing another batch.
- Verify Clear Board and Begin Voyage postconditions instead of assuming that
  a void API call succeeded, with bounded retries and explicit fallback to the
  existing stable Element path.
- Retain the visual Voyage Planner only as the final temporary fallback, while
  logging API state, board signatures, transfer results and every fallback
  reason through `VOYAGE_DPB_*` and `VOYAGE_UI_FALLBACK` markers.

## 0.23.141 - 2026-08-04

- Quarantine Chart-area transitions immediately after the first successful
  Bathysphere portal interaction, avoiding object, inventory and GUI reads
  until a nonzero new area hash is stable for two consecutive refreshes.
- Suppress repeated portal interactions for eight seconds and permit only one
  bounded retry before the 30-second `ChartAreaTransitionTimeout` recovery
  stop.
- Restrict automatic Capsule direct-UI inspection to Voyage areas so Hub and
  Chart maps no longer read the extraction window on every context refresh.
- Mark coroutine resume faults once, request a controlled stop and rebuild
  stale or faulted coroutine/task state on the next Start instead of emitting
  an endless resume-exception loop.

## 0.23.140 - 2026-08-04

- Reset Valerie alternate-approach recovery state across task start, stop,
  area exit, Voyage start, UI/dialog success, target replacement and completed
  recovery so stale hard-timeout timestamps cannot stop a fresh attempt.
- Preserve the originating task in controlled-stop markers when the progress
  watchdog has already been cleared.
- Transfer normal personal-stash loot with one explicit Ctrl+right-click per
  cycle, confirm the inventory delta on a fresh cycle and always release input
  state after the click.
- Run Charts already present in inventory before probing or withdrawing Chart
  Storage, with explicit source-selection diagnostics.
- Delay pre-Voyage Chart storage until Begin Voyage has actually succeeded,
  preventing startup Charts from being deposited and withdrawn again.

## 0.23.139 - 2026-08-04

- Replace Valerie alternate-approach movement success tracking with sampled
  convergence and target-crossing checks; rotate candidates after four
  seconds without real progress and bound the complete recovery to six
  candidates, two pather reloads and 25 seconds.
- Keep Chart Device portal entry in its confirmed-Descend state when a portal
  interaction reopens the Chart UI, close the reopened window and reacquire
  the portal instead of treating a transient loaded snapshot as a new map.
- Reset stale custom Chart local IDs when the native device is empty and a
  different inventory Chart is available, allowing the current Chart to load
  instead of waiting forever with Descend disabled.
- Add two bounded empty-device recoveries, a 30-second Chart Device transaction
  timeout and explicit watchdog progress for load, Descend, portal appearance
  and the first portal interaction.

## 0.23.138 - 2026-08-04

- Start object, inventory, visible-UI and Chart Storage cache windows after
  their scans finish so a slow DPB memory read cannot immediately expire its
  own cache and force another full scan.
- Add per-stage milliseconds to `CONTEXT_REFRESH_TIMING` and a scoped
  `INVENTORY_REFRESH_TIMING` marker for diagnosing client-specific memory and
  UI slowdowns without guessing from complete task-cycle duration.
- Stop forcing complete object, inventory and 1200-element UI scans on every
  Chart Storage cycle; use the direct Chart Storage wrapper and inventory-only
  snapshots for withdrawal confirmation, completion and source failure.
- Replace the Voyage Stash pre-check full refresh with the same cached
  inventory-only path while preserving full refreshes for actual UI and
  pathfinder recovery boundaries.
- Cache normal stash Chart summary scans independently for 350 ms, preserving
  item counts while avoiding repeated `StashUi.AllItems` enumeration on every
  context refresh.

## 0.23.137 - 2026-08-04

- Replace the exhausted-Lantern coverage, reserved-Capsule and breadcrumb
  rescue routes with an immutable return chain built only from Capsules the
  player actually approached within 25 units during exploration.
- Order the return chain from the nearest cached Capsule toward the Capsule
  nearest the Voyage spawn, keeping the spawn Capsule as the final attempt.
- Interact with each cached Capsule exactly once, inspect only new chat lines
  for the `Capsule ... not lit` response and advance permanently to the next
  cached target when rejected or when no UI outcome is observed.
- Preserve the existing Capsule UI, Call Bathysphere and extraction handling
  after a cached Capsule opens successfully, while bypassing all Lantern
  coverage checks for the return chain and its confirmed UI.
- Bound each approach with convergence tracking and at most two pather reloads,
  bound the complete return chain to 120 seconds and create a controlled
  `VoyageCapsuleReturnExhausted` recovery stop when no cached Capsule succeeds.
- Add `CAPSULE_CACHE_*` and `CAPSULE_RETURN_*` diagnostics for discovery,
  confirmation, frozen ordering, movement, one-shot interaction, chat outcome,
  recovery, success and exhaustion.

## 0.23.136 - 2026-08-04

- Accelerate Chart Storage reads by advancing through already-active Main and
  Sub tabs in the same observation cycle, starting page-signature validation
  immediately and shortening the stable-empty confirmation window.
- Move normal stash items through DPB `InventoryControlWrapper.FastMove`,
  confirm transfers from a fresh inventory snapshot and retain bounded
  Ctrl-right-click fallback for rejected API transfers.
- Reduce verified stash open, tab-switch and default per-item transfer delays;
  migrate only the previous untouched `250-380 ms` defaults so custom user
  timings remain unchanged.
- Prepare and warm up the pather before Deepwater chest movement, ignore path
  failures during that window and adopt a pather loaded by an earlier task
  instead of reloading it a second time.
- Rate-limit chest path-failure counting and perform at most two pather/mover
  recoveries before an actually unreachable chest is skipped, preventing a
  newly discovered `Message in a Bottle` from being discarded within a few
  hundred milliseconds of entering a Chart Area.
- Add `CHEST_PATHFINDER_*`, `CHEST_PATH_RECOVERY` and
  `STASH_ITEM_TRANSFER_ISSUED` diagnostics for runtime verification.

## 0.23.135 - 2026-08-04

- Add a mandatory final Capsule approach for exhausted-Lantern Voyages when a
  live targetable Capsule is within 150 units, keeping the target locked even
  when the Lantern coverage model rejects its target or segment.
- Fall back through path-validated local steps of 40, 30, 20 and 12 units and
  allow the final interaction-ring candidates outside modeled coverage.
- Measure real convergence toward the locked Capsule, reload the pather at
  most twice after four seconds without progress and retain those recovery
  counters across route retries.
- Enforce a 30-second final-approach timeout with a controlled
  `MandatoryCapsuleReturnTimeout` recovery stop instead of rotating the same
  Capsule forever.
- Add `CAPSULE_FINAL_APPROACH_*` diagnostics for target lock, local steps,
  progress, recovery and timeout decisions.

## 0.23.134 - 2026-08-04

- Accelerate pre-Voyage Chart Storage deposits with a persistent fast-deposit
  page state: after the initial page selection, each cycle Ctrl-clicks one
  fresh inventory Chart without clicking Main/Sub tabs again.
- Confirm each Chart transfer from a later inventory snapshot, remove the
  per-item forced context refresh and reduce modifier-click input latency.
- Require two bounded transfer rejections before treating a storage page as
  full and advancing to the next page.
- Make confirmed `0/x` Lantern state an unconditional return trigger in both
  Chart Areas and Voyages, cancelling all pending chest and ground-loot work.
- Remove zero-Lantern chest grace, valuable-loot exceptions and their GUI
  settings; Chart Areas immediately prioritize extraction while Voyages
  immediately prioritize mandatory Capsule rescue.
- Add `CHART_STORAGE_FAST_DEPOSIT_*` and
  `ZERO_LANTERN_MANDATORY_RETURN` diagnostics for runtime verification.

## 0.23.133 - 2026-08-04

- Recover when portal interaction reopens an empty Chart Device after Descend:
  preserve the confirmed Descend state, close the stale UI, reacquire the
  Bathysphere portal and retry with a bounded interaction cadence.
- Distinguish a genuinely loaded reopened Chart Device from the empty-device
  recovery case and include UI recovery counts in the portal timeout marker.
- Make mover fallback search incremental while preserving the immediate direct
  path, limiting expensive path candidates to six checks per task cycle.
- Make ground loot, Bottle opening, personal Voyage Stash withdrawal, visual
  respawn confirmation and Sulphur pickup cooperative across task cycles.
- Refresh context on tiered object, inventory and UI schedules, and queue
  manual snapshots for the next normal cycle instead of refreshing in the
  message handler.
- Remove blocking mouse sleeps and the retained Descend `Element`; reacquire
  UI state before input and log `TASK_CYCLE_SLOW`/`CONTEXT_REFRESH_TIMING` for
  remaining runtime stalls.
- Add portal reopen, incremental action and one-action-per-cycle diagnostics
  without slowing normal direct movement or exploration traversal.

## 0.23.132 - 2026-08-04

- Rebuild Voyage Planner placement as a cooperative state machine: each task
  cycle submits at most one Chart move or Begin Voyage click, then reacquires
  a fresh stable DPB UI snapshot on the following cycle for confirmation.
- Apply the same incremental flow to the DPB inventory and visual fallback;
  remove the previous nine-click batches, nested polling loops and retained UI
  elements across placement actions.
- Rebuild Chart Storage withdrawal so each cycle transfers one raw Chart,
  returns to TaskManager, then confirms the item disappeared from a freshly
  resolved page before selecting the next Chart or storage page.
- Select Chart Storage main/sub tabs and stabilize each page incrementally;
  never click both tabs or poll a page to completion inside one task cycle.
- Evaluate one Voyage short-escape or obstacle-detour candidate per cycle,
  returning immediately after any movement submission or candidate rejection.
- Submit one Capsule inventory FastMove per cycle and confirm source removal
  from a fresh inventory snapshot instead of polling and falling through to a
  second Ctrl-click action in the same cycle.
- Make Chart Device load, Descend, portal entry and capacity recovery return
  immediately after input; deferred UI closure now runs on the next cycle.
- Make utility flasks and Enduring Cry own the cycle when they submit input,
  use at most one utility flask per cycle, and suppress nonessential utility
  actions during mandatory zero-Lantern Capsule extraction.
- Scan live Capsule objects in one bounded manual pass without LINQ sorting,
  keep a locked target through 3.5 seconds of transient ObjectManager loss,
  and require a tighter 20-25 unit final interaction range.
- Preserve the mandatory live-Capsule lock within 150 units and extend its
  reacquire grace so a transient object snapshot cannot rotate immediately to
  a distant registry anchor.
- Add `VOYAGE_RUNNER_STATE`, incremental Chart move, Chart Storage issue and
  confirmation markers with `OneActionPerCycle=True` diagnostics.

## 0.23.131 - 2026-08-03

- Separate new-Capsule acquisition from locked-target maintenance and fresh
  interaction snapshots during mandatory zero-lantern extraction.
- Retain the exact locked Capsule by target id, anchor and area through the
  approach, stabilization and interaction stages even after it has been
  recorded in the rescue cycle target history.
- Reuse the live object already resolved by the rescue loop and allow a short
  snapshot reacquire window before rotating a genuinely missing target.
- Add locked-target retained, rejected and reacquire markers, plus explicit
  search mode and tried-filter details to Capsule live-scan logging.

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
