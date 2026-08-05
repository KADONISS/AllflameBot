# Feature Change Checklist

- [ ] Read `AGENTS.md` and the feature module context.
- [ ] Identify the owning task and task-order impact.
- [ ] Identify state read/write ownership.
- [ ] Preserve DPB compatibility and single-DLL packaging.
- [ ] Define primary action, fallback and postcondition.
- [ ] Check loop, retry, stop and movement-ownership behavior.
- [ ] Add or update a fixture/test when the logic can run without DPB.
- [ ] Update docs, ADR, knowledge or changelog when needed.
- [ ] Build or record why build was unavailable.
- [ ] Review `git diff` and generated-file exclusions.
