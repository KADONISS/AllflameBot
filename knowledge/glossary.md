# Glossary

## Purpose

Define project terms so agents do not confuse UI boards, world terrain and
runtime phases.

## Audience

New developers, reviewers and AI agents.

## Update when

Update when a term gains a new meaning or a new domain concept is introduced.

## Terms

- **Chart:** A playable Chart Area and, in planning context, an inventory item
  placed on the Voyage Planner board.
- **Voyage:** The playable Deepwater Voyage world area, not the planner board.
- **Voyage Planner:** The DPB/game UI containing inventory chart slots, board
  slots, Clear Board and Begin Voyage controls.
- **Allflame Capsule:** A high-priority Voyage object that can affect traversal
  and extraction safety.
- **Lantern:** Coverage/resource object that gates safe Voyage movement and
  exploration.
- **Bathysphere:** The Voyage transport/extraction interaction.
- **AreaHash:** The runtime identity used to invalidate stale map, object and
  movement state.
- **Terrain snapshot:** A copy of DPB terrain/cache/walkability data for an area.
- **Frontier:** A candidate exploration target selected from layout, topology or
  terrain data.
- **Postcondition:** Fresh evidence that an input action changed the intended
  game/UI state.
- **Handled cycle:** A task returned ownership for the current task-manager
  cycle, not necessarily completion of its full workflow.
- **Recovery:** A bounded attempt to restore progress without ending the bot.
- **Terminal stop:** A call path that requests `BotManager.Stop(false)`.
