# UI and Input Boundary

## Purpose

Define how UI discovery, input and postcondition verification are layered.

## Audience

Agents changing chart planning, Bathysphere, stash or warning dialogs.

## Update when

Update when a UI adapter, fallback order or input method changes.

## Depends on

[`docs/modules/ui.md`](../../docs/modules/ui.md),
[`rules/ui-interaction.md`](../../rules/ui-interaction.md) and
[`docs/dpb-compatibility.md`](../../docs/dpb-compatibility.md).

## Adapter order

1. Stable DPB `Element` nodes when exposed and validated.
2. Native DPB control or inventory APIs when available.
3. Scoped coordinate input derived from the current window/control.
4. Screenshot or text fallback with a fresh visual confirmation.

Every action must release held input and identify its verification mode in the
log. A visual match is evidence of a visible control, not proof that the game
accepted the action; the next state observation remains authoritative.
