# Workflow: Release

## Purpose

Produce a verified plugin DLL and a traceable source commit.

## Audience

Agents and developers releasing the plugin.

## Update when

Update when release tooling or deployment contract changes.

## Steps

1. Check branch, status and version markers.
2. Build `AllflameBot.csproj` for x64 `net48` with matching DPB dependencies.
3. Verify DLL metadata, `Data` content and `3rdparty.json`.
4. Run available static, unit and live checks.
5. Update changelog/release notes.
6. Review ignored/generated files and `git diff`.
7. Commit, push and report SHA, branch and build result.

If the external DPB assemblies are missing, stop at evidence collection and
report the build blocker; do not fabricate a release result.
