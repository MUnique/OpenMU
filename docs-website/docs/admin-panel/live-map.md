---
title: Live map
sidebar_position: 11
description: Watch players and monsters on a map in real time and moderate from there.
---

# Live map

**Navigation:** the map icon in the [server list](servers.md), or
`/gameServer/{id}/` for the map overview of a game server.

The live map is a graphical, real-time representation of what happens on a game
map. It is implemented in WebGL (three.js) and updated through Blazor's
JavaScript interop as the server publishes events.

## Map overview of a server

Clicking the map icon of a game server opens a card per map which the server
hosts, each with its current player count. Clicking a card opens the live map of
that map.

If the server hosts no maps yet — because it is not started — the page says so.

## What you see

* **Players and NPCs** as objects on the terrain of the map
* **Movements** as they happen
* **Attacks and skills**, including area skill animations
* **Kills** — objects disappearing when they die

Clicking an object shows its details: name, id, position and, for players, the
character level.

## The player list

Next to the map is the list of the players who are currently on it, with these
actions per player:

| Action | Effect |
|---|---|
| **Select** (click the row) | Highlights the player on the map and shows their details |
| **Disconnect** | Closes the connection of that player |
| **Temporarily ban** | Sets the account state to `TemporarilyBanned` and disconnects the player |
| **Follow** | The map view follows this player while they move; click again to stop |

This makes the live map the fastest way to deal with a player who is behaving
suspiciously: watch them, follow them, and ban them from the same screen.

:::note[Bans made here]
A temporary ban set from the live map changes the state of the account. Review
and lift it on the [Accounts page](accounts.md).
:::

## Filtering the monsters

A busy map shows hundreds of monsters. To find some of them — for example the
golden monsters of an invasion — use **Filter monsters** above the player list:

1. Choose how the name is compared: **contains**, **starts with** or
   **ends with**.
2. Type the text. The comparison ignores the case.

While a text is set, only the monsters whose name matches stay on the map.
Players and the other NPCs, like merchants and guards, are always shown.
Monsters which spawn later are filtered as well.

Below the filter is the list of the matching monsters with their position. It is
refreshed every second. Clicking a row selects the monster, like a click on the
map. Clear the text, or click the button next to it, to show all monsters again.

For example, *contains* `dragon` finds Budge Dragon, Red Dragon and
Golden Dragon, and *starts with* `golden` finds the monsters of a Golden
Invasion.

## Slow or lost connections

The game never waits for the browser of an admin who watches the live map. While
the connection to the browser is lost, or when the browser falls far behind, the
updates are dropped, and the map is rebuilt with the current state as soon as the
browser can keep up again. Watching a map from a slow connection therefore can't
slow down the game for the players.

## Availability

| Deployment | Live map |
|---|---|
| All-in-one (and from source) | Rendered by the admin panel itself |
| Distributed | The link leads to the reverse-proxied map application of the game server container |

## Planned

Some ideas which are not implemented yet: zooming in on players, display of all
skill animations and of active magic effects, health status, an overview of
several maps on one page, a view of public chats, and game master actions such
as dropping items or starting automated events.
