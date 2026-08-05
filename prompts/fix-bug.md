# Prompt: Fix a Bug

Use `workflows/bug-investigation.md`. Start from logs/source evidence. Classify
the failure as observation, input, path, UI, state, external or lifecycle. Find
the task arbitration and recovery path. Prefer a state/strategy correction over
a new stop. Add a regression fixture or diagnostic event, then validate the
postcondition and inspect all affected fallbacks.
