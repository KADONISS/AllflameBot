# DPB UI Analysis Skill

## Use when

Analyzing Element trees, warning dialogs, chart placement, stash UI or input
fallbacks.

## Read first

`docs/modules/ui.md`, `architecture/boundaries/ui-input.md`,
`rules/ui-interaction.md` and `docs/dpb-compatibility.md`.

## Method

Identify the window/control scope, stable element, native API, coordinate fallback
and visual fallback. Require a fresh postcondition and input cleanup.

## Output

UI evidence, selected mode, fallback order, postcondition and compatibility risk.
