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

1. Validated native DPB control/action APIs for semantic state changes that
   must work while the client process is covered.
2. Stable DPB `Element` nodes when a native action is unavailable or its
   postcondition is not yet trusted.
3. Scoped coordinate input derived from the current window/control.
4. Screenshot or text fallback with a fresh visual confirmation.

Native Voyage Planner reads may run in shadow mode while Element remains the
authoritative planner snapshot. Native board actions stay opt-in until the
native and stable snapshots agree across fresh consecutive observations.

Every action must release held input and identify its verification mode in the
log. A visual match is evidence of a visible control, not proof that the game
accepted the action; the next state observation remains authoritative.
