---
title: Gens
sidebar_label: Gens
sidebar_position: 3
description: The two families Duprian and Vanert, which players of Season 6 can join at the gens npcs, and their fights in the battle zone.
---

# Gens

The gens are the two families of Season 6, **Duprian** and **Vanert**. A player
joins one of them at its npc, and the game client then shows the gens mark and
rank next to the name of the character. The members of the two gens fight each
other in the battle zone for contribution points, which give them their rank.

:::note
The gens system is implemented step by step. Joining and leaving a gens, the
battle zone, the contribution for kills, the ranking and the party, guild and
alliance rules work. The gens chat and the monthly rewards are not implemented
yet.
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

The option of the gens ranking reward at the npc is answered, but a member
isn't eligible for a reward yet, because the rewards aren't implemented.

The npc shows the contribution points of a member, when it talks to the npc of
its own gens. The gens info window (key `B`) shows the gens, the rank, the
position in the ranking and the contribution, and how many points are missing
for the next rank.

## Battle zone

The battle zone is Vulcanus by default, which the game client marks as
*(Battle)* in its warp list. Only gens members can enter it, by the warp list or
by a warp gate.

When a member kills a member of the other gens in the battle zone:

* the killer doesn't become an outlaw, and attacks don't start the self-defense;
* the killer gets contribution points, and the victim loses some (never below 0).

The points depend on the level difference:

| Level of the killer | Killer gets | Victim loses |
|---|---|---|
| more than 50 below the victim | 7 | 3 |
| 11 to 50 below the victim | 6 | 3 |
| up to 10 below or above the victim | 5 | 3 |
| 11 to 30 above the victim | 3 | 3 |
| 31 to 50 above the victim | 2 | 1 |
| more than 50 above the victim | 1 | 1 |

When the victim has a better rank, the killer gets a bonus of 3, 4 or 5 points
for a difference of 1, 2 or 3 ranks and more. A killer with the rank *Knight*
(9) or lower gets 1, 2 or 3 bonus points instead. A victim without contribution
points gives no points.

Repeated kills of the same victim are limited: from the third kill within an
hour, the killer is warned, and from the sixth one, the kills don't change the
contribution points anymore. The count starts again an hour after the last kill.
The counts are saved, so they don't start again when one of the players logs
out.

Kills during a duel don't change the contribution points.

:::note
A character which isn't a gens member, but is in the battle zone when it logs
in (e.g. because the battle zone was configured differently before), isn't
moved out of it.
:::

## Ranks and ranking

There are 14 ranks. The lower ranks only depend on the contribution points, the
higher ones also on the position in the ranking of the own gens:

| Rank | Title | Requirement |
|---|---|---|
| 14 | Private | 0 points |
| 13 | Sergeant | 500 points |
| 12 | Lieutenant | 1500 points |
| 11 | Officer | 3000 points |
| 10 | Guard Prefect | 6000 points |
| 9 | Knight | 10000 points |
| 8 | Superior Knight | 10000 points, position 201 to 300 |
| 7 | Knight Commander | 10000 points, position 101 to 200 |
| 6 | Baron | 10000 points, position 51 to 100 |
| 5 | Viscount | 10000 points, position 31 to 50 |
| 4 | Count | 10000 points, position 11 to 30 |
| 3 | Marquis | 10000 points, position 6 to 10 |
| 2 | Duke | 10000 points, position 2 to 5 |
| 1 | Grand Duke | 10000 points, position 1 |

The ranks by points change right after a kill. The ranking of each gens is
calculated when the server starts and then every two hours by default; with the
same points, the member which joined first is ranked higher.

## Parties, guilds and alliances

* Members of different gens can't form a party.
* No party can be formed in the battle zone, and a player leaves its party when
  it enters the battle zone.

The original game also limits the guilds and alliances to the gens. Because
these rules affect all players, also the ones which don't care about the gens,
they are only active when they're configured:

* Only gens members can create a guild, and a player can only join the guild of
  a guild master of the same gens.
* Only the masters of guilds of the same gens can form an alliance.

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
| Battle zone map numbers | 63 | The maps of the battle zone. |
| Kill contributions | see above | The points of a kill by the level difference. |
| Rank bonuses | see above | The bonus points for a victim with a better rank. |
| Low rank bonus from rank | 9 | The rank from which a killer gets the lower bonus. |
| Minimum victim contribution | 1 | The points which a victim needs, so that the killer gets points. |
| Abuse warning kill count | 3 | The kills of the same victim from which the killer is warned. |
| Abuse limit kill count | 6 | The kills of the same victim from which they give no points. |
| Abuse reset time | 60 minutes | The time after which the count of kills starts again. |
| Ranks | see above | The ranks and their requirements. |
| Ranking interval | 2 hours | The interval of the ranking. |
| Allow party with other gens | no | Whether members of different gens can form a party. |
| Allow party in battle zone | no | Whether parties can be formed in the battle zone. |
| Guild requires gens | no | Whether creating and joining a guild requires the gens, like in the original game. |
| Alliance requires same gens | no | Whether an alliance requires the same gens of the guild masters, like in the original game. |

The memberships (`GensMember`) and the counts of the kills (`GensAbuse`) are
kept in their own tables, which are part of the backup of the accounts.
