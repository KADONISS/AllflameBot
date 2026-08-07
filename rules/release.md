# Release Rules

## Purpose

Define what must be true before a DLL is handed to DPB.

## Audience

Agents and developers preparing releases.

## Update when

Update when the project target, artifact or deployment contract changes.

## Rules

- Build x64 `net48` with matching external references.
- Confirm project, runtime and mover version markers.
- Confirm `Data` content is copied and `3rdparty.json` is coherent.
- Review changelog and release notes for user-visible changes.
- Never include local settings, logs, secrets, DLL or PDB in a source commit.
- Confirm the deployable DLL has access to the matching `Data` content; a
  DLL-only asset is not self-contained unless that content is already deployed.
- For a public release, confirm the `v<version>` tag resolves to the source
  commit and the release assets have matching versions and recorded SHA256
  values.
- Record commit SHA, tag, release URL and build result.
