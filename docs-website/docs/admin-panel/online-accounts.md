---
title: Online accounts
sidebar_position: 6
description: See who is playing, disconnect players and stop offline sessions.
---

# Online accounts

**Navigation:** *Online accounts* — route `/logged-in`

This page answers the question "who is on my server right now?" and lets you kick
somebody.

## Logged-in accounts

The first table lists every account which is currently logged in:

| Column | Meaning |
|---|---|
| Login name | The account which is logged in |
| Server id | The game server the account is playing on |
| Action | **Disconnect** — closes the connection of that account |

**Disconnect** is the tool of choice for a player who is misbehaving right now.
Combine it with a ban on the [Accounts page](accounts.md) if the player should not
come back.

## Active offline players

The second table lists the **offline sessions** — players who left their
character in the game world without being connected, for example to keep a
personal store open.

| Column | Meaning |
|---|---|
| Login name | The account of the offline session |
| Server id | The game server the session runs on |
| Started at | When the offline session started (UTC) |
| Action | **Stop** — ends the offline session |

Stopping an offline session removes that character from the game world, which
also closes its store.

:::tip[Player counts]
The total number of connections per server is shown on the
[Servers page](servers.md), including a total over all game servers.
:::
