# Known Limitations

## Purpose

Keep unresolved or externally constrained facts visible to future agents.

## Audience

Agents planning changes and reviewers assessing residual risk.

## Update when

Update when a limitation is reproduced, resolved or superseded.

## Current limitations

- The DPB binary is external; this repository cannot prove every runtime API
  behavior.
- Terrain cache completeness and world-coordinate transform require live
  validation.
- Chart terrain planning exists, while Voyage target generation remains a
  rolling frontier implementation.
- Several settings are exposed in configuration/UI but need runtime consumer
  audits before they can be treated as active policy.
- `AllflameBot.cs` is a large monolith with static shared state.
- There is no automated test project or CI pipeline.
- `3rdparty.json` metadata does not fully describe the references declared by the
  project file.
- Documentation historically lagged source releases; the new docs are the
  maintained architecture layer, not a guarantee that old README sections are
  current.
