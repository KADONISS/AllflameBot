# Allflame Bot

DPB 3.29 bot for the Allflame/Deepwater league mechanic.

## Release 0.23.93

Version 0.23.93 distinguishes a real reward held on the cursor from the
transient cursor-open state seen immediately after using a Bottle. Accept
image matching is now mandatory until a non-empty cursor item is observed.

## Release 0.23.92

Version 0.23.92 recognizes the Message in a Bottle Accept reward dialog with
CopyFromScreen template matching, clicks Accept, and physically places the
cursor-held reward into a size-compatible free inventory slot. Reward
classification also detects an item that replaces the Bottle under the same
local id.

## Release 0.23.91

Version 0.23.91 opens Message in a Bottle with a physical inventory
right-click and confirms that the bottle was consumed before classifying the
resulting reward. This removes the unreliable high-level UseItem path.

## Release 0.23.90

Version 0.23.90 completes Secret Bottle processing with a configured personal
or guild stash source. The task periodically scans the selected tab in the
Sovereign Hub, counts inventory plus stash Bottle quantities, withdraws only
when the combined threshold is met, verifies every transfer, and then runs the
existing open, price, keep, sell, and fallback pipeline. It operates
independently of Chart and Voyage automation toggles.

## Release 0.23.89

Version 0.23.89 adds the Secret settings tab and bounded Message in a Bottle
processing. Bottles are opened from the live DPB inventory at the configured
threshold; rewards are priced, valuable or uncertain items are kept, and only
confirmed low-value reward IDs are offered to a DPB-confirmed NPC Sell UI.
Vendor, pricing, transfer, or confirmation failures conservatively fall back
to the normal stash workflow.

## Release 0.23.88

Version 0.23.88 prevents untraversed Capsule steering outside the active
lantern-safe radius. Capsule search now delegates back to safe frontier
exploration until the target is covered, preventing dark-area deaths.

## Release 0.23.87

Version 0.23.87 adds native 800x600 grayscale templates for Respawn,
Call Bathysphere, warning OK, Descend, and both Begin Voyage states. Existing
1280x800 and 1920x1080 templates remain available, while Voyage Chart fallback
continues to use green markers only and never restores the `L:xx` signal.

## Release 0.23.86

Version 0.23.86 removes the `L:xx` text-derived recovery signal from Voyage
Chart counting. DPB inventory remains the primary source, while the visual
fallback now counts only compact green Chart-marker components to prevent
text fragments from inflating the available Chart count.

## Release 0.23.85

Version 0.23.85 prevents the Lantern `V` fallback from opening the Voyage
league panel after the visible Lantern counter reaches zero. Placement input
now requires a positively observed remaining charge and is guarded again
immediately before the key event to cover counter changes during approach.

## Release 0.23.84

Version 0.23.84 caps each ground-loot item at three consecutive rejected
interactions. A failed item is marked unlootable for the current loot session,
removed from chest loot debt, and receives a 12-second cooldown record. The
bot logs `GROUND_LOOT_GIVE_UP` with the next action and no longer retries the
same visible item indefinitely.

## Release 0.23.83

Version 0.23.83 makes zero-Lantern chest grace strictly safety-bounded. Chest
movement, opening, Bottle orbit, and every loot movement/click now abort as
soon as the player loses Lantern aura or the target falls outside recorded
coverage. A cached Bathysphere return that remains outside aura for two seconds
switches to the reverse Lantern/breadcrumb route.

## Release 0.23.82

Version 0.23.82 explicitly accepts `Voidborn Reliquary Key` drops by name or
`VoidbornVaultKey` metadata. The key is treated as mandatory high-priority
chest loot even though DPB reports its item class as `VaultKey`.

## Release 0.23.81

Version 0.23.81 changes notification tables to a boxed two-column layout with
Unicode borders, matching the compact reference style while preserving the
existing values and live-message update behavior.

## Release 0.23.80

Version 0.23.80 formats the live Discord dashboard and standalone alerts as
consistent monospace `METRIC | VALUE` tables. The session table now also shows
the current status, last event, warning, and stop details when available.

## Release 0.23.79

Version 0.23.79 moves the image-only Call Bathysphere path outside the
disabled high-level DPB UI gate. A visually confirmed Call button now reaches
the bounded click and disappearance-confirmation flow at Voyage extraction.

Essence objects are explicitly denied by name and metadata before chest-route
registration, and again by chest selection/priority, so AllflameBot never
navigates to or interacts with an Essence as a reward chest.

## Release 0.23.78

Version 0.23.78 adds a two-snapshot shortfall failsafe to Voyage planning.
When the cleared right-side pool consistently contains fewer than nine
clickable Charts, the visual count replaces the stale progress estimate, the
planning table closes, and Chart Area collection resumes instead of reopening
Plan Voyage indefinitely.

## Release 0.23.77

Version 0.23.77 secures a traversed Allflame Capsule before normal Voyage
chest routing can consume the remaining Lanterns. A known Capsule is pursued
immediately through short frontiers that preserve Lantern placement, while an
unknown exit triggers a bounded reserve search at 20 percent of the starting
Lantern count (minimum 4, maximum 16).

The zero-Lantern 10/15-second grace now retains only chests and active chest
loot clusters within 120 units of an observed placed Lantern. Unsafe queued
chests are discarded and rechecked before movement/opening, so the grace
cannot send the character into darkness.

## Release 0.23.76

Version 0.23.76 embeds separate Call Bathysphere templates captured at
1280x800 and 1920x1080. Every visual candidate is compared with both templates,
and the best template is recorded in the log while the existing two-frame
confirmation, adaptive thresholds and bounded recovery remain unchanged.

## Release 0.23.75

Version 0.23.75 rewrites the Call Bathysphere button path to use
`Graphics.CopyFromScreen` template matching only. It requires two stable
frames, adapts correlation from 0.60 to 0.50 and finally 0.40 after each five
visual attempts, and confirms the click from two consecutive frames where the
button has disappeared. DreamPoeBot UI text and element detection are disabled
for this action; bounded click retries and three reopen recovery cycles remain.

## Release 0.23.70

Version 0.23.70 removes Windows Graphics Capture and its helper process,
settings and shared-memory integration. Every image-recognition feature now
uses the original `Graphics.CopyFromScreen` implementation and the original
client-coordinate click path again.

The compact Voyage UI and empty Plan Voyage fixes from 0.23.69, together with
the later Capsule, chest and pickup behavior, remain unchanged.

## Release 0.23.69

Version 0.23.69 fixes PrepareVoyage looping after Plan Voyage opens with no
Charts. The visual detector now accepts compact clients down to 700x500
instead of silently rejecting the observed 800x600 frame. An empty Voyage
pool is confirmed on two fresh frames, overrides a stale ready estimate,
closes the planning table and returns control to the Chart runner.

## Release 0.23.66

Version 0.23.66 separates Capsules that were merely discovered from Capsules
the player actually passed. Return, reserve and emergency selection now accept
only previously traversed Capsules. While Lanterns remain, the runner
proactively visits newly discovered Capsules after chest movement, opening and
loot, but before placing another Lantern. A blocked visit is bounded to ten
seconds and is retried after a twenty-second cooldown instead of blocking the
run or being forgotten permanently.

The Call Bathysphere visual fallback now uses the normal 0.60 correlation
threshold during recovery and scans a narrower vertical button region. This
rejects the false-positive location seen in the 13:54 log while preserving API
UI confirmation and trying another traversed Capsule after three unconfirmed
open attempts.

## Release 0.23.65

Version 0.23.65 adds a configurable, bounded chest grace period when the
Lantern counter reaches 0/x. Charts and Voyages may finish already-discovered
chests and their eligible drops for up to the configured number of seconds,
then the existing return or Bathysphere extraction flow resumes. The grace
ends immediately when no chest or loot work remains, and a value of zero keeps
the previous immediate-exit behaviour.

## Release 0.23.64

Version 0.23.64 adds independent toggles and priorities for both new 3.29.1
Deepwater reward chests, Sunken Opulence and Sunken Gems. Sunken Opulence is no
longer treated as ordinary Sunken Loot. It also adds selections for Diviner's,
Arcanist's and Operative's Strongboxes found inside Charts and Voyages.

Unique and Gem pickup categories now have independent minimum Divine-value
settings. A value of zero preserves the previous pick-up-all behaviour for the
enabled category. A positive threshold requires a valid price quote and logs
the accept/reject decision as `PICKIT_VALUE`.

## Release 0.23.63

Version 0.23.63 adds a guarded image-recognition fallback for the visible
`RESURRECT AT CHECKPOINT` button. It is armed only while the character is
reported dead and the DPB panel flag is false or an API attempt has already
failed. Two consecutive visual matches must meet the correlation threshold
and agree on location before the bot clicks. The template is embedded in the
DLL, and visual detection/click/timeout decisions are recorded in the log.

## Release 0.23.62

Version 0.23.62 verifies each Chart Ctrl+Click with a fresh visual Available
count before treating that Chart as placed. An input event alone no longer
counts as success: the same Chart is retried once, then planning stops and
retries safely if its removal cannot be observed.

## Release 0.23.61

Version 0.23.61 performs both high-confidence Clear Board snapshots inside a
single `PrepareVoyageTask.Run` call. This prevents an intermediate task-state
reset from discarding snapshot one and interacting with Valerie, which closed
the visible Voyage table before Chart Ctrl+Click placement could execute.

## Release 0.23.60

Version 0.23.60 keeps ownership of an already-detected Voyage planning UI
while the two-snapshot Clear Board guard is active. A transient missed Begin
Voyage match now retries against the same open UI for up to five seconds
instead of falling through to Valerie interaction and closing the table before
Chart Ctrl+Click placement can run.

## Release 0.23.59

Version 0.23.59 stores `AllflameBot.json` in the current Windows user's
`Documents\AllflameBot` directory so replacing or updating the DPB folder no
longer resets bot configuration. On first use, an existing
`DPB\Settings\AllflameBot.json` is copied to Documents without overwriting an
existing Documents configuration. Startup logs `SETTINGS_STORAGE` with the
selected path and migration status; an inaccessible Documents folder falls
back to the legacy DPB settings path.

## Release 0.23.58

Version 0.23.58 uses DreamPoeBot 0.3.29.15 AreaGraphs topology rooms as
optional discovery targets before the existing Chart and Voyage frontiers.
Only named rooms with plausible bounds and a valid DPB path are accepted.
Known chests and loot still preempt exploration, Capsule search does not use
topology routing, and any blocked or stalled topology target immediately falls
back to the previous frontier planner. The Extra tab contains a switch to
disable topology routing without changing the established navigation.

## Release 0.23.57

Version 0.23.57 adds a constrained recovery threshold for the Call
Bathysphere button after repeated accepted Capsule interactions. A live,
targetable Capsule inside Lantern coverage is no longer blacklisted solely
because the visual UI probe missed three times. Route and total-search
failsafes now remove a stale Unusable mark before forcing the nearest live
Capsule, and rejection logs distinguish blacklist state from actual Lantern
coverage.

## Release 0.23.56

Version 0.23.56 prevents a transient, low-confidence Begin Voyage match from
turning the fixed Clear Board offset into a click on a Voyage Board slot. Clear
Board now requires two consecutive snapshots with at least 0.50 correlation,
no more than 12 pixels of anchor drift, and no more than 0.02 scale drift. If
the anchor remains unstable for five seconds, the UI is closed and reopened
without clicking the board.

## Release 0.23.55

Version 0.23.55 prevents an infinite `RESPAWN_WAIT|PanelNotOpened` loop.
After a 1.5-second UI grace period it calls the respawn API even when DPB's
panel flag remains false. It tries checkpoint up to three times and forces
town resurrection after 15 seconds, while preserving the existing `/boat`
exit for failed one-entry encounters.

## Release 0.23.54

Version 0.23.54 marks a remembered Capsule unusable after three blocked-route
events or 10 seconds without meaningful distance progress. It clears the
confirmed anchor and immediately prefers the nearest live targetable Capsule.
A 30-second total search failsafe disables remembered breadcrumb routing and
delegates live Capsule discovery to the Voyage planner.

## Release 0.23.53

Version 0.23.53 re-arms Voyage Stash withdrawal when reconciliation ends a
Voyage chain with fewer than 9 Charts. This prevents `OpenChartDeviceTask` from
looping on `InventoryCharts=0` while stored Charts are still available.

## Release 0.23.52

Version 0.23.52 extends the Valerie interaction watchdog to `PrepareVoyageTask`.
If interaction reports success but neither dialogue nor the Voyage UI appears
within 0.9 seconds, the bot blacklists its current position for 10 minutes and
moves to another reachable approach. The unsafe east-side approach sector is
now blacklisted relative to Valerie, so the protection follows translated boat
coordinates instead of relying only on `{1198,457}`.

## Release 0.23.51

Version 0.23.51 blacklists the last bad Valerie approach near `{1198,457}`.
OpenChartDevice now waits only 900 ms for a successful NPC interaction to
produce the `Chart Area` dialogue; if no effect appears, it blacklists the
current player position for ten minutes and immediately rotates to another
path-reachable approach point.

## Release 0.23.50

Version 0.23.50 prevents the spent Allflame Capsule
(`SummonExtractionObjectFallback`) from being classified as the returning
Bathysphere. Stage 9 now accepts only an actual extraction object and releases
control for a Call retry when a real Bathysphere remains non-targetable past
the configured wait timeout.

## Release 0.23.49

Version 0.23.49 prevents Voyage exploration from repeatedly selecting a
frontier that is already inside the arrival radius, and adds a shared Valerie
recovery for Voyage, Chart identification, and Chart Device interactions. If
the direct NPC approach stalls or Valerie is not highlighted, the bot now
rotates through path-reachable approach points around her before falling back
to boat staging.

## Release 0.23.48

The Stash tab now exposes `Charts per withdrawal` (1-60, default 20).
Both the configured personal Voyage Stash and native Chart Storage use this
batch target. A new batch is still withdrawn only after the previous batch
has been consumed completely.

## Release 0.23.47

`Allflame Ember of the Gilded` is no longer classified as walk-over Gold from
its `GoldAllflamePack` metadata and is picked up through the normal item
interaction path. Walk-over Gold also has a bounded near-target retry count;
if a supposed Gold item remains on the ground, the bot falls back to click
pickup instead of holding the loot task indefinitely.

Version 0.23.46 detects a full 60/60 Voyage inventory from a persistently
disabled Descend button and switches to Voyage planning.

## Release 0.23.45

Chained Voyage planning now preserves the stable Begin Voyage button anchor
while placing the next nine Charts. A low-confidence visual false-positive
can no longer replace that anchor and prevent the second Voyage from starting.

## Voyage loop (0.21.0)

Voyage is implemented as a separate ten-stage state machine: enter the
post-Begin-Voyage Bathysphere, lock the immutable spawn, build dynamic
macro-sectors with micro-frontiers, discover/open/loot configured chests,
track real Lantern state, send inventory loot through remembered Capsules,
call and actively wait for the Bathysphere at `0/x`, extract to The Sovereign,
withdraw Collect Loot, and run the normal stash policy.

The Voyage map is not forced into nine fixed rectangles. A completed local
frontier grows the next sector from the furthest reachable position. Capsule
positions are registered while exploring, so the extraction phase routes to
the nearest known reachable Capsule. The configured free-square threshold
pre-empts exploration immediately; transfer-at-boundary is an additional
proactive trigger rather than a gate. Version 0.20.11 verifies each inventory
transfer, sends a staged batch only once, confirms the Call UI transition,
evicts stale Capsule anchors, and delegates unknown-Capsule searches to the
unvisited rolling frontier planner. Voyage exhaustion requires a stable real
`0/x` counter across multiple snapshots (never a counted `V` press). Repeated
unverified Send or Call operations stop the bot with an explicit recovery
reason. A failed item transfer pauses and retries while space remains, but
still stops safely if the inventory is full or extraction is required.

Chart-device entry keeps the custom UI latched after a Chart leaves inventory,
rotates through every Descend click strategy, and stops safely after seven
unsuccessful attempts. Valerie/path failures re-stage through the boat centre.
The mover keeps a stable approach point and uses shorter waypoints near loot,
NPCs, and the Bathysphere to reduce overshoot.

Voyage frontier and detour candidates are raycast against the current walk
map before movement. A blocked long frontier is clipped 28 units before the
first collision; a collision within 55 units rejects that direction. World
interactions with Valerie, Bathyspheres, and Allflame Capsules finish the
current movement and clear held input first. Ground-loot interaction remains
unsynchronized so combat looting does not pause unnecessarily.

The rolling planner also probes 72 units to both sides of every candidate
heading. A wall on only one side applies a penalty larger than heading
continuity, so the runner turns back into open terrain instead of travelling
parallel to the map edge. Capsule transfers use `Inventory.FastMove` first and
verify against a refreshed inventory snapshot. Non-critical transfer failure
backs off for 30 seconds and resumes exploration; the bot stops only when the
inventory is full or extraction requires preserving unsent loot.

## Chart Area loop

Version `0.17.1` runs the Chart Area loop actively. Travel with `/boat`, stage
at the central deck to the right of Chart Storage, and
Chart Storage inspection are mandatory workflow steps rather than settings.

Before opening the Chart Device, the bot checks every Chart in the main
inventory. If any Chart is unidentified, it talks to Valerie, discovers the
available `Identify` conversation option from the live NPC dialog, uses it,
and blocks Chart loading until the inventory reports those Charts identified.

The bot builds a lightweight layout-frontier plan from the immutable Chart
spawn and combines it with dynamic chest discovery. Four cardinal branches,
three alternate lanes and progressively wider rings avoid blocking the bot
on a single wall without scanning the complete terrain in one frame. It no
longer creates DPB's GridExplorer, so unrelated
Harvest/Incursion/Affliction/Breach Tower segmentation is neither run nor
logged by Allflame Bot.

The in-area loop is explicitly phased: a selected chest pauses Lantern
placement, then runs Navigate, Open and chest-only Loot. Patrol resumes after
that chest queue is empty. Lantern exhaustion pre-empts every chest phase and
replays the recorded movement breadcrumbs in reverse to the Bathysphere.

On every new Chart Area hash, the bot explicitly reloads ExilePather before
requesting a route. If repeated path probes still fail, it uses short rotating
direct movement steps while periodically retrying the navmesh reload.

For each of the three main Chart Storage categories, the bot starts at page
1 and opens the next page only when the current page is full. It then
withdraws from the newest used page back toward page 1, stopping as soon as
the main inventory contains 20 Charts. The observed page counts are saved in
`AllflameBot.json`.

For the first Chart Area it talks to Valerie and selects `Chart Area`. Once
the Chart Device is available, it inserts a Chart and presses `Descend`.
The custom Descend button is detected from a built-in grayscale image
template with multi-scale matching inside the PoE client, so moving the game
window does not invalidate the click position.
After Descend, it waits for the targetable Bathysphere portal and enters it
instead of continuing to click the closed device UI.
Subsequent runs open the device directly through the Bathysphere. In a Chart
Area it records configured reward chests while patrolling and placing Golden
Lanterns. The seasonal counter is treated as remaining Lanterns: `7/7` is
full, while a confirmed `0/7` triggers the highest-priority return phase.

Selected Deepwater chests are saved in a persistent per-area route as the
explorer reveals them. Allflame Mover calculates a walkable path to the
closest pending chest, and the target remains queued if it temporarily leaves
object range. Placed Lantern positions are numbered in placement/discovery
order. When Lanterns are exhausted, the exact outward breadcrumbs are replayed
once in reverse before moving to the remembered extraction Bathysphere.

Before opening a selected reward chest, the bot snapshots existing WorldItem
ids. Only new ids appearing afterward inside the configured chest radius are
eligible, so items already dropped by monsters are ignored. Chest types have
independent 0-100 priorities; higher priority opens first and distance breaks
ties.

When an item is accepted while its in-game filter label is visible, that
decision remains sticky until its WorldItem disappears. The Loot tab exposes
drop-settle, click cadence and failed-pickup retry delays. After the chest
queue is empty, an explicit handoff releases the old route and selects an
escape frontier, preventing the previous post-loot idle loop.

The package includes `Allflame Mover`, so the Chart Area loop does not depend
on TheThief or Heist Mover.

The `Stash` tab configures personal/guild loot tabs, stashing after
`Collect Loot`, and an inventory free-square threshold. Text settings are
saved immediately while typing.

Logs are written to `Logs/AllflameBot`.

The bot stops automatically when both the current inventory and configured
Voyage Stash tab contain no Charts. During normal stash policy runs it also
stops when every configured loot tab rejects the same remaining item after
the per-tab transfer retries, which is treated as full loot storage.

## Prices and profit (0.21.0)

The `Prices` tab maintains a persistent Divine-denominated item price cache.
The default `Allflame` league adapter reads the current PoE 1 economy
endpoints, converts Chaos values through the current Divine Orb rate, and
stores the result in:

`Settings\AllflameBot\prices.json`

Automatic updates default to every 12 hours. The previous cache remains
active if an update fails, and the newest ten cache backups are retained.
Fixed overrides use the format:

`Item Name=1.5; Another Item=0.25`

An override is a unit price in Divine Orbs and wins over downloaded data.
Loot is counted only after the corresponding WorldItem is confirmed removed,
so interaction retries do not duplicate quantity or profit. Stack size is
included in the valuation.

Confirmed `Dead Man's Sulphur` is valued from the same currency price feed and
is included as a separate component:

`Session profit = Chest loot + Sulphur value`

If the feed has no Sulphur quote, a per-unit fixed override can be supplied,
for example `Dead Man's Sulphur=0.00002074`.

## Discord notifications (0.23.17)

The `Notifications` tab accepts a Discord webhook and provides a test button.
Immediately after the bot enters its started state, it creates one Discord
message containing Session Summary, Remaining Resources, Items Found, and
Total Profit, then retains its message ID. All subsequent changes use Discord's
webhook message-edit endpoint to update that same dashboard.

The live dashboard is refreshed when a Chart/Voyage starts or completes, a
chest opens, loot or Sulphur is confirmed, a warning changes, and when the bot
stops. Rapid chest/loot changes are debounced and all HTTP writes are
serialized so an older snapshot cannot overwrite a newer one. Low Chart,
death-abandon, valuable-item, and price-data events request a dashboard refresh
without creating standalone messages.

Valuable items have an optional exception: enable `Tag @everyone for valuable
items` and set `Minimum value (Divine Orbs)` to the desired threshold (for
example `1`). After a qualifying pickup is confirmed, the bot sends one
standalone `@everyone` alert containing the item, quantity, estimated value,
and source. Discord `allowed_mentions` is restricted to `everyone` for this
payload and the option is disabled by default.

The dashboard contains duration, completed Chart Areas and Voyages,
success rate, deaths, opened chests, remaining resources, the highest-value
items, total profit, profit per hour, price age, and the bot stop reason.
Sulphur is shown as a confirmed session total and priced separately from chest
loot; it is not linked to a `Chart #` card.
Webhook URLs are never written to the compact Allflame log.

## Collect Loot adapter (0.23.13)

Collect Loot no longer depends on DPB exposing seasonal inventory `114`.
The bot first keeps the supported `RewardUi`/inventory fast paths, then uses
an independent UI-tree adapter that requires all of these signals before it
clicks anything:

- the visible window title is exactly `LOOT`;
- the candidate is the portrait 12-column, 18-24-row item grid below it;
- item elements come from that grid's own `DictionaryIdElements`;
- each item centre is still inside the confirmed grid.

The adapter opens page 1, Ctrl+clicks and verifies every item removal, then
does the same for page 2. It only marks the surface loot collected after two
empty reads on both pages. A failed item transfer is retried twice, after
which the UI is closed and reopened instead of clicking untrusted positions.
Useful live-test records are `COLLECT_RAW_UI`, `COLLECT_RAW_TAB`,
`COLLECT_RAW_ITEM`, and `COLLECT_RAW_UI_WAIT`.

## Running

1. Start DreamPoeBot and allow the `3rdParty` content loader to compile.
2. Select `Allflame Bot` in the bot list.
3. Enable `Deepwater Probe` if a full diagnostic dump is also desired.
4. Start the bot.
5. Stop DPB and inspect the newest file in `Logs/AllflameBot`.

The observer log is intentionally compact. `Deepwater Probe` remains the
source for full UI trees and complete object snapshots.

## DPB developer guidance

These rules come from the DPB developer notes and apply to new AllflameBot
logic:

- `Utility.BroadcastMessage(sender, id, params dynamic[] input)` invokes
  `Message` on Content, the current Routine, the current Mover, and enabled
  Plugins, in that order. The call remains on DPB's `MainCoroutine` thread;
  message handlers must be short and must never perform blocking or expensive
  work.
- Message ids and custom input/output types are an integration contract.
  Keep ids documented, validate inputs with `TryGetInput<T>`, and use named
  outputs when reading `Message` results. Shared custom types must have the
  same data type and constructor in every communicating component.
- Treat every bot cycle as a state-machine step: inspect the current snapshot,
  perform at most one action, then return to the task manager. Do not use a
  long `for`/`foreach` loop for movement, combat, looting, or interaction;
  returning lets the bot react to newly appearing threats and changed UI state.
- `LokiPoe.ObjectManager` exposes a nearby-object snapshot, limited to roughly
  210 units. Movement can invalidate objects from that snapshot, so reacquire
  objects after meaningful movement and never assume a previously captured
  object remains valid.
- Object ids are unique within a zone. Cache only the small set of properties
  needed by the current task, mark processed ids, and skip them on later ticks.
  Limit snapshot refreshes with a stopwatch or equivalent cadence limiter.
- Avoid repeated property reads and heavy LINQ chains on live DPB objects.
  DPB resolves object properties from game memory through the object address;
  `Where`, `OrderBy`, and chained projections can become expensive in dense
  areas. Prefer one bounded scan, lightweight cached records, and explicit
  distance comparison when ordering is required.
- Currency Exchange UI changes are asynchronous. After selecting currencies or
  quantities, wait with `Coroutines.LatencyWait()` and verify the corresponding
  selected value before continuing. Place, cancel, and retrieve operations
  must also be bounded and confirmed from the resulting order state.

# Native Deepwater support (0.23.20)

- Requires DreamPoeBot 0.3.29.11 or newer; built and verified against 0.3.29.12.
- Chart Device detection now uses `DeepwaterChartUi.IsOpened`.
- Device contents are verified through the native Deepwater inventory.
- Descend is issued only after `IsDescendEnabled`, using native `Descend()`.
- Image/geometry Descend detection remains only as a fallback.
- Movement and Enduring Cry use DPB skill readiness checks.
- Per-process log filenames prevent collisions when multiple clients share one DPB installation.
