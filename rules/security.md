# Security Rules

## Purpose

Prevent credentials, private runtime data and unsafe automation changes from
entering the repository.

## Audience

All contributors and AI agents.

## Update when

Update when a new secret, network service or privileged action is introduced.

## Rules

- Never commit Discord webhook URLs, credentials, local settings or logs with
  personal identifiers.
- Keep runtime settings under ignored `Settings/` or the user documents path.
- Validate local paths before file operations.
- Do not add network calls without documenting endpoint, timeout and failure
  behavior.
- Do not weaken input cleanup or process-hook safety.
