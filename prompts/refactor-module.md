# Prompt: Refactor a Module

Read `AGENTS.md`, `architecture/overview.md`, the module context and relevant
ADRs. Map current callers and shared state before moving code. Preserve the
single-DLL contract and public DPB entry points. Do not change behavior and
structure in one opaque step. Add characterization tests or fixtures where
possible, then update dependency and module documentation.
