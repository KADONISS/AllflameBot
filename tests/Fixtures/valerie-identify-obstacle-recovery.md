# Valerie Identify Obstacle Recovery Fixture

## Source assumptions

- Sanitized from `AllflameBot_2026-08-11_13-55-19_pid-2428.log`.
- DPB `0.3.29.32`, AllflameBot `0.23.167`, area hash `3576452263`.
- This is a log-review fixture; it is not an automated live-client test.

## Failure sequence

```text
VALERIE_APPROACH_BLACKLIST|State=CandidateSkipped|Reason=KnownBoatNpcEastSector|Candidate={2578, 457}|Npc={2544, 457}
MOVER_PATH_REPLAN|Target={2544, 491}|AreaHash=3576452263
VALERIE_APPROACH_RECOVERY|State=Arrived|Target={2544, 491}|TargetDistance=6.3|NpcDistance=36.0|NpcInRange=True
CHART_IDENTIFY|Action=Interact|Unidentified=5|Distance=36.0|Success=True
CHART_IDENTIFY|Action=Interact|Unidentified=5|Distance=26.0|Success=True
BOT_STOP|Source=UserStop
```

The interaction result confirms that input was accepted, not that the
inventory changed. The same five unidentified Charts remained while the task
issued 387 interactions before the user stopped the bot.

## Expected recovery sequence

```text
CHART_IDENTIFY_POSTCONDITION|State=Pending|Action=Interact|InputSent=True|UnidentifiedBefore=5|AreaHash=3576452263
CHART_IDENTIFY_POSTCONDITION|State=Failed|Reason=NoInventoryProgress|UnidentifiedBefore=5|UnidentifiedAfter=5
VALERIE_APPROACH_BLACKLIST|State=Added|Reason=IdentifyNoInventoryProgress|Position={player}|Radius=5.0|AreaHash=3576452263
BOT_INPUT_CLEANUP|Source=ChartIdentifyNoInventoryProgress|...
CHART_IDENTIFY_RECOVERY|Reason=NoInventoryProgress|Action=BlacklistAndAlternateApproach
VALERIE_APPROACH_REPLAN|Reason=IdentifyNoInventoryProgress|AreaHash=3576452263
```

The blocked point is the runtime `LokiPoe.Me.Position`, not the inferred
approach target `{2544, 491}`. The blacklist is valid only for the current
`AreaHash`, and the existing Chart Device blacklist radius remains unchanged.

## Acceptance criteria

- A successful interaction must be followed by a lower unidentified-count
  postcondition before the task treats it as completed.
- Three unchanged observations or three seconds without inventory progress
  create one radius-five blacklist entry and force an alternate approach.
- Candidate points inside the blacklist are rejected by path selection.
- A fresh area hash removes the old block, and the recovery does not add a new
  terminal stop path.
