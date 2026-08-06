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
3. Scoped coordinate input derived from the current window/control. Screen
   coordinates from DPB elements are converted to client coordinates before
   background messages are posted.
4. Background `PostMessage` input when the game is covered or not foreground;
   physical mouse input is used only when the target point is foreground and
   uncovered.
5. Screenshot or text fallback with a fresh visual confirmation.

## Covered-window behavior

`BackgroundUiInput` keeps a stamp of the game HWND, client origin, client size
and DPI. A cached UI snapshot or coordinate plan is rejected when that stamp
changes. Visible captures use the desktop only when the sampled client region
is uncovered; covered captures use `PrintWindow` as a best-effort fallback.

`PostMessage` confirms that messages were queued, not that the game accepted
them. Control modifiers are included in mouse messages and paired key
messages, and every action must still verify the next UI state.

Every action must release held input and identify its verification mode in the
log. A visual match is evidence of a visible control, not proof that the game
accepted the action; the next state observation remains authoritative.
