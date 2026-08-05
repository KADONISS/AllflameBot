# Required Checks

## Purpose

List the minimum evidence expected before a change is committed.

## Audience

Agents and reviewers.

## Source checks

- Confirm the relevant task order and phase guards.
- Confirm all direct DPB API calls and version assumptions.
- Search for existing fallback, retry, stop and recovery behavior.
- Check whether settings exposed in the UI are actually consumed.
- Review the full diff for unrelated changes and generated files.

## Runtime checks

- Build against the matching DPB and Newtonsoft.Json assemblies.
- Verify the plugin assembly name and target framework.
- Exercise the changed path with structured logs when live runtime access exists.
- Verify action postconditions on a fresh observation cycle.
- Confirm held input is released on every failed or completed interaction.

## Documentation checks

- Update the relevant architecture or module context when responsibility changes.
- Update a business rule or ADR when behavior or a decision changes.
- Update `CHANGELOG.md` for user-visible behavior.
- Keep secrets, runtime logs, binaries and local settings out of commits.

## Known limitations

No automated test project or CI pipeline currently exists. If a check cannot be
run because DPB binaries or a live client are unavailable, record that fact
instead of claiming success.

## Update this file when

Update it when build, test, release or evidence requirements change.

## Dependencies

Depends on `rules/testing.md`, `rules/release.md`, `docs/dpb-compatibility.md`
and `workflows/release.md`.
