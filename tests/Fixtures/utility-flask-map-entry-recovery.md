# Utility Flask Map-Entry Recovery

## Source assumptions

- Bot version: `0.23.167`.
- DPB build: `0.3.29.31`.
- Area: `DeepwaterEncounter` or `DeepwaterEncounterVoyage`.
- This is a sanitized source/log regression fixture, not a live-client test.

## Expected success path

```text
UTILITY_FLASK_MAP_ENTRY|AreaHash=12345|CooldownSeconds=5|Postcondition=MovementMayContinue
UTILITY_FLASK_SKIP|Reason=MapEntryCooldown|AreaHash=12345|Postcondition=MovementMayContinue
UTILITY_FLASK_BEGIN|Task=UseLifeFlaskTask|Phase=ChartArea|AreaHash=12345|Slot=2|InputCleanup=BeforeCall|Mode=Native
UTILITY_FLASK_INPUT_READY|AreaHash=12345|Slot=2|InputReleasedBefore=True|CleanupElapsedMs=2|Postcondition=InputCleared
UTILITY_FLASK_END|Task=UseLifeFlaskTask|Phase=ChartArea|AreaHash=12345|Slot=2|Success=True|ElapsedMs=40|InputReleasedAfter=True|Postcondition=DeferredFreshSnapshot|Error=
```

The first utility-flask call is delayed for five seconds after the area hash
changes. A pending Chart transition skips the call and returns control to the
ordered movement/recovery tasks.

## Expected slow-call recovery

```text
UTILITY_FLASK_CIRCUIT|Reason=SlowNativeCall|AreaHash=12345|Slot=2|ElapsedMs=1200|ThresholdMs=1000|Action=DisableForArea|Postcondition=MovementMayContinue
UTILITY_FLASK_SKIP|Reason=DisabledForArea|AreaHash=12345|Postcondition=MovementMayContinue
```

The slow call disables only utility flasks for the current area. Emergency Life
Flask handling and movement tasks remain available, and no terminal stop is
expected.
