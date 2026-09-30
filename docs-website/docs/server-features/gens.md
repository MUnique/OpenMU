---
title: Gens
sidebar_label: Gens
sidebar_position: 3
description: The two families Duprian and Vanert, which players of Season 6 can join at the gens npcs.
---

# Gens

The gens are the two families of Season 6, **Duprian** and **Vanert**. A player
joins one of them at its npc, and the game client then shows the gens mark and
rank next to the name of the character.

:::note
The gens system is implemented step by step. Joining and leaving a gens works.
The battle zone, the contribution for kills, the ranking and the monthly rewards
are not implemented yet.
:::

## Joining and leaving

A player joins a gens by talking to its npc and choosing to join:

* **Gregory** (Duprian) stands in Lorencia.
* **Conrad** (Vanert) stands in Noria.

To join, the character has to:

* have at least the configured level (50 by default),
* not be in a party,
* not be a member of a guild — join the gens first, then the guild,
* not be a member of a gens already, and
* have waited the configured time since it left its last gens (no waiting time by default).

A new member starts with the configured contribution points (10 by default) and
the lowest rank, *Private*.

A player leaves its gens at the npc of its own gens. A guild master can't leave.
Leaving resets the contribution and the rank.

The npc shows the contribution points of a member, when it talks to the npc of
its own gens. The gens info window (key `B`) shows the gens, the rank and the
contribution.

## Configuration

The gens system is configured by the **Gens system** plugin in the admin panel.
When the plugin is deactivated, players can't join or leave a gens.

| Setting | Default | Meaning |
|---|---|---|
| Minimum level | 50 | The minimum level of a character to join a gens. |
| Starting contribution | 10 | The contribution points of a new member. |
| Starting rank | 14 | The rank of a new member, from 1 (highest, *Grand Duke*) to 14 (lowest, *Private*). |
| Rejoin wait time | 0 | The time a character has to wait after leaving a gens, until it can join again. |
| Duprian npc number | 543 | The npc of the Duprian gens. |
| Vanert npc number | 544 | The npc of the Vanert gens. |

The memberships are kept in their own table (`GensMember`), which is part of the
backup of the accounts.
