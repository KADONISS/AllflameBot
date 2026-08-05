# Prompt: Generate Tests

Read `rules/testing.md` and isolate the pure behavior first. Use real log,
terrain, UI or inventory fixtures when the bug depends on evidence. Do not mock
DPB APIs in a way that proves only the mock. Separate unit, adapter contract and
live-client checks. Report skipped checks and external dependency requirements.
