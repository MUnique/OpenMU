---
title: Duel
sidebar_label: Duel
sidebar_position: 5
description: The 1 on 1 fight between two players, either in the duel arena with spectators or on the map where the two players stand.
---

# Duel

A duel is an agreed 1 on 1 fight between two players. The winner is the one who
first reaches the configured score — the duel doesn't end with the first death,
a killed duelist respawns and fights on.

Two variants exist, and the server owner picks one:

* **Duel arena** — the duelists are teleported into a free area of the duel
  arena, where they fight undisturbed and other players can watch them. That's
  how the duel works in the original game since Season 4.
* **Current map** — the duel takes place where the two players are standing,
  with all other players and monsters around, like in the seasons before the
  duel arena existed.

## Starting a duel

A player requests the duel from another player in the game client, and the
other player accepts or refuses it. The server only starts the duel when:

* both players are on the same map,
* both have at least the configured minimum level,
* both have at least the configured entrance fee, which is taken from both when
  the duel starts,
* neither of them is a player killer of the second stage,
* neither of them is in a mini game, in a guild war, in an active self-defense
  or has an npc dialog open, and
* in the duel arena variant, one of the duel areas is free.

Five seconds after both duelists are in place, the fight begins.

## During the duel

* A kill of the opponent raises the score of the killer. Only kills between the
  two duelists count.
* The killer of the opponent doesn't become an outlaw, and the hits between the
  duelists don't start the self-defense. Everybody else keeps that protection,
  also against a player which is in a duel.
* Kills during a duel don't change the [gens](gens.md) contribution points.
* For clients without the shield system (the versions 0.75 and 0.95d), the
  damage between the duelists is reduced to 60 %.
* The duel is cancelled when a duelist leaves the map of the duel, logs out, or
  requests to stop it.

The duel ends when a duelist reaches the configured maximum score. Both players
and the spectators are then informed about the winner.

## Duel arena

The duelists are teleported to the two gates of a free duel area, so each duel
has its own part of the arena. Other players watch a duel as spectators:

* **Doorkeeper Titus** in Vulcanus shows the duel watch window with the
  running duels, from which a player joins one of them as spectator.
* A spectator is teleported to the spectator gate of that duel area and is
  invisible for the duelists.
* A spectator sees the score and the health of both duelists.

Ten seconds after the duel ended, the duelists and the spectators are
teleported to the configured exit gate — Vulcanus by default — or to the
safezone of the map if no exit is configured.

## Duel in the current map

The duel takes place where the two players stand, so:

* nobody is teleported, neither for the duel nor after it;
* a killed duelist respawns at the normal spawn point of the map and can walk
  back into the fight;
* other players and monsters can attack the duelists, and the duelists can
  attack others — with the usual consequences, like becoming an outlaw;
* the duel can't be watched by spectators, because everybody around sees it
  anyway, so the duel watch window of the client stays empty;
* the number of duels which run at the same time is not limited by the number
  of configured duel areas.

## Configuration

The duel is configured in the admin panel, in the game configuration, as
**Duel configuration**:

| Setting | Default | Meaning |
|---|---|---|
| Variant | Duel arena | Where a duel takes place: in the **duel arena**, or in the **current map** of the duelists. |
| Maximum score | 10 | The number of kills with which a duelist wins the duel. |
| Entrance fee | 30000 | The money which is taken from both duelists when the duel starts. |
| Minimum character level | 30 | The level which both duelists need. |
| Maximum spectators per duel room | 0 | How many players can watch one duel of the duel arena variant. The shipped configuration doesn't set it, so it has to be raised before spectators can join a duel. |
| Exit | Vulcanus | The gate to which the players are teleported after a duel in the duel arena. Without it, they are teleported to the safezone. |
| Duel areas | 4 areas of the duel arena | The gates of the duelists and of the spectators, per area. Only used by the duel arena variant. |

:::note
Changing the variant only affects duels which start afterwards. Existing
installations keep the duel arena variant, which was the only one before this
setting existed.
:::
