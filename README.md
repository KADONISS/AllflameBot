# Allflame Bot

DPB 3.29 bot for the Allflame/Deepwater league mechanic.

## Current status

This repository builds one DreamPoeBot plugin assembly targeting .NET Framework
4.8, x64 and C# 7.3. The runtime is an ordered `ObserverTask` pipeline that
reads DPB state, performs bounded actions and verifies postconditions on fresh
cycles.

## Start here

- [`AGENTS.md`](AGENTS.md) - repository rules and required reading.
- [`.agents/index.md`](.agents/index.md) - context routing by task type.
- [`architecture/overview.md`](architecture/overview.md) - current boundaries.
- [`docs/getting-started.md`](docs/getting-started.md) - setup and deployment.
- [`docs/runtime-lifecycle.md`](docs/runtime-lifecycle.md) - lifecycle and task cycles.
- [`docs/configuration.md`](docs/configuration.md) - settings ownership.
- [`docs/troubleshooting.md`](docs/troubleshooting.md) - evidence-first diagnosis.
- [`docs/dpb-compatibility.md`](docs/dpb-compatibility.md) - external DPB contract.
- [`workflows/release.md`](workflows/release.md) - build and release checks.

## Build and deploy

1. Make the matching `DreamPoeBot.exe` and `Newtonsoft.Json.dll` available at
   the paths declared by [`AllflameBot.csproj`](AllflameBot.csproj).
2. Build the project for x64 `net48`.
3. Verify `AllflameBot.dll` and `Data/uniqueArtMapping.default.json` are present
   in the output.
4. Copy the plugin assembly and `Data` content into the DPB `3rdParty` location.
5. Confirm the plugin loads before enabling unattended automation.

The [`3rdparty.json`](3rdparty.json) manifest and project file are part of the
deployment contract. Do not commit DPB binaries, DLL/PDB output, logs, settings
or secrets.

## Runtime guidance

- Treat every bot cycle as one state-machine step: observe, perform at most one
  bounded action, yield, then verify.
- Preserve the registered task order; it is the runtime arbitration mechanism.
- Reacquire DPB objects and UI after meaningful movement or state changes.
- Route movement through `AllflameMovementCoordinator` and preserve safety,
  capsule, extraction and loot priorities over exploration.
- Prefer refresh, reacquisition, route invalidation and coroutine recovery over
  adding automatic stop behavior.
- Log adapter mode, input result and postcondition for consequential actions.

See [`docs/dpb-developer-guidance.md`](docs/dpb-developer-guidance.md) for the
expanded DPB integration rules.

## Runtime operation

1. Start DreamPoeBot and allow the `3rdParty` loader to compile the plugin.
2. Select `Allflame Bot` and the `Allflame Mover` when movement is required.
3. Verify initialization, version, mover and process-hook log entries.
4. Start the bot and inspect `Logs/AllflameBot` when diagnosing behavior.

## Documentation maintenance

Update [`docs/maintenance.md`](docs/maintenance.md) whenever code changes
responsibility, state ownership, external dependencies, UI fallback, planner
coordinates, recovery behavior, settings or release behavior.

Historical release entries are maintained in [`CHANGELOG.md`](CHANGELOG.md) and
are intentionally not duplicated in this README.
