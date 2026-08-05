# Error Handling Rules

## Purpose

Keep transient DPB/UI/path failures recoverable and terminal conditions explicit.

## Audience

Agents changing exception, retry, watchdog or stop logic.

## Update when

Update when recovery or terminal-stop policy changes.

## Rules

- Classify errors as observation, input, path, UI, state, external network or
  lifecycle failures.
- Recover by refreshing, reacquiring, invalidating, retrying or rebuilding when
  the state remains safe.
- Do not catch and ignore exceptions without logging context.
- Do not add automatic stops for convenience.
- If a terminal stop remains necessary, log reason, detail, active task and
  recovery attempts.
- Always release held input in failure paths.
