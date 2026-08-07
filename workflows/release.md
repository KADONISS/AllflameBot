# Workflow: Release

## Purpose

Produce a verified plugin DLL, a traceable source commit and, when requested,
a traceable public release.

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
6. Review ignored/generated files and `git diff`; keep DLL, ZIP and PDB files
   out of the source commit.
7. Commit and push the source branch. Record the commit SHA.
8. For a public release, create an annotated `v<version>` tag on that commit
   and push the tag.
9. Create the GitHub Release for the tag and upload `AllflameBot.dll`. Upload
   `AllflameBot-<version>.zip` when a self-contained DLL plus `Data` package is
   required.
10. Verify the remote tag resolves to the source commit, the release targets
    that tag, every asset is uploaded successfully and each asset SHA256 is
    recorded.
11. Report the branch, commit SHA, tag, release URL, artifact paths, metadata
    and build result.

If the external DPB assemblies are missing, stop at evidence collection and
report the build blocker; do not fabricate a release result.
