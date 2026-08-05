# Getting Started

## Purpose

Explain how to build and deploy the plugin without relying on stale release
notes.

## Audience

Operators and developers setting up a local DPB environment.

## Update when

Update when the supported DPB version, dependency paths or deployment process
changes.

## Depends on

[`AllflameBot.csproj`](../AllflameBot.csproj), [`3rdparty.json`](../3rdparty.json),
[`docs/dpb-compatibility.md`](dpb-compatibility.md) and
[`workflows/release.md`](../workflows/release.md).

## Setup

1. Close DreamPoeBot before replacing or building the plugin.
2. Place the repository under the DPB `3rdParty` tree or adjust the project
   reference paths to the local DPB installation.
3. Make sure the matching `DreamPoeBot.exe` and `Newtonsoft.Json.dll` exist at
   the paths required by the project file.
4. Build the x64 `net48` project.
5. Verify `AllflameBot.dll` and `Data/uniqueArtMapping.default.json` are present
   in the output/deployment location.
6. Start DPB, load the bot and select `Allflame Mover` when movement is needed.

## First runtime checks

- Confirm the log contains the current bot version and successful initialization.
- Confirm the selected mover and process hook state.
- Confirm settings are stored in the user settings location, not the repository.
- Run a low-risk UI or area transition and inspect the structured log before
  enabling unattended operation.

## Update this document when

Update it whenever an operator would need a different setup or verification
step.
