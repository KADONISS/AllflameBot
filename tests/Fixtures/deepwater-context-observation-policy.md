# Deepwater Context Observation Policy

## Source assumptions

- Runtime is .NET Framework 4.8 x64.
- DPB build is `0.3.29.31`.
- This is a sanitized source/log regression fixture, not an automated live
  client test.

## Expected Chart Area evidence

```text
CONTEXT_REFRESH_STAGE|RefreshId=10|Stage=VisibleUi|State=Skipped|Reason=DeepwaterScoped|Policy=NativeAndOnDemandFallback|AreaId=DeepwaterEncounter|AreaHash=101
CONTEXT_OBJECT_TARGETED|Target=Bathysphere|Mode=TargetedLookup
DEEPWATER_COUNTER_SOURCE|Source=Native|Native=True|Current=6|Maximum=7
```

No `CONTEXT_REFRESH_STAGE|Stage=VisibleUi|State=Begin` or
`UI_SCAN_FAILED` is expected for the same refresh. Chart Device reads remain on
the native adapter path, while scoped Element/visual fallback is invoked only
by the owning action task.

## Expected Voyage evidence

```text
CONTEXT_REFRESH_STAGE|RefreshId=11|Stage=VisibleUi|State=Skipped|Reason=DeepwaterScoped|Policy=NativeAndOnDemandFallback|AreaId=DeepwaterEncounterVoyage|AreaHash=202
CAPSULE_NATIVE_SNAPSHOT|Opened=True|CallEnabled=True
CONTEXT_OBJECT_TARGETED|Target=Allflame Capsule|Mode=WaitingForBathysphere.TargetedLookup
```

`CapsuleUiVisible`, `CallBathysphereVisible` and
`SendToSurfaceVisible` are derived from the fresh native snapshot. If native
state is unavailable, the task owns the bounded Element or visual probe and
must log its input mode and postcondition.

## Area-scoped breaker evidence

```text
CONTEXT_REFRESH_UI_CIRCUIT|RefreshId=20|Stage=VisibleUi|AreaId=SomeArea|Complete=False|AreaHash=303|Reason=Incomplete|Policy=DisabledUntilAreaChange|DisabledUntil=AreaChange
CONTEXT_REFRESH_STAGE|RefreshId=21|Stage=VisibleUi|State=Skipped|Reason=AreaCircuitBreak|Policy=DisabledUntilAreaChange|AreaHash=303
CONTEXT_REFRESH_UI_CIRCUIT_RESET|PreviousAreaHash=303|NewAreaHash=404|Action=AllowGlobalVisibleUiScan
```

The breaker is diagnostic and recoverable. It does not stop the bot and it
does not carry visible text from the previous area into current UI flags.
