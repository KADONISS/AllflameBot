# Compatibility History

## Purpose

Track external DPB compatibility discoveries separately from release history.

## Current entries

- The project references external `DreamPoeBot.exe` and `Newtonsoft.Json.dll`.
- Runtime terrain and UI semantics require validation against the deployed DPB
  build.
- On 2026-08-21, the release build in `DPB1_0_3_29_32` resolved
  `DreamPoeBot.exe` as `0.3.29.39`; the compatibility marker and release notes
  record that exact dependency identity.

## Update trigger

Add a dated entry when a DPB/client change is tested and confirmed.
