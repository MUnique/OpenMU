---
title: Accounts
sidebar_position: 5
description: Search, create, ban and edit player accounts.
---

# Accounts

**Navigation:** *Accounts* — route `/accounts`

The account list shows all accounts ordered by login name, with their state and
e-mail address.

## Searching

The search box above the list filters by login name. The list is paged and
loaded on demand, so it stays usable with a large number of accounts.

## Creating an account

The **Create** button at the bottom opens a dialog which asks for the login name,
the password and (optionally) an e-mail address. The password is hashed by the
server — you never store it in plain text.

This is mainly useful for creating a game master account or a test account on a
server which does not offer registration.

## Account states

The state of an account decides how it is treated when it logs in:

| State | Meaning |
|---|---|
| `Normal` | A normal player account |
| `Spectator` | Invisible to players and monsters |
| `GameMaster` | A game master account — this is what unlocks the game master [chat commands](chat-commands.md) |
| `GameMasterInvisible` | Game master, invisible to players and monsters |
| `Banned` | Permanently banned; the account cannot log in |
| `TemporarilyBanned` | Temporarily banned |

## Banning a player

To ban an account, open it with **Edit** and set its **state** to `Banned` (or
`TemporarilyBanned`), then save.

If the player is currently online, the ban does not kick them by itself — use the
[Online accounts](online-accounts.md) page to disconnect them.

## Editing an account

**Edit** opens the generic edit page of the account. From there you reach
everything that belongs to it: the characters with their stats, inventory and
skills, the vault, and the account's settings.

:::warning[Technical view]
This is the [generic edit page](game-configuration.md#the-generic-edit-pages),
i.e. a direct view of the data model. It is powerful and it is easy to create
inconsistent data with it.
:::

### Creating a character

**Create** below the characters of an account asks only for the **name** and the
**character class**. The character is then set up with the same data as one which
a player creates in the game:

* it gets the stat attributes of its class (level, strength, agility, …) with
  their start values,
* it starts on the home map of its class, at a random spawn position,
* it gets an empty inventory and the default key configuration,
* it takes the first free character slot of the account.

The name has to match the **character name regex** of the
[game configuration](game-configuration.md). The admin panel can create
characters of every class, including the ones which can't be created in the game
(e.g. a second class or a locked class). An account which already has the
maximum number of characters can't get another one.

The game additionally runs the *Character created* [plugins](plugins.md) for a
new character, which give it the initial items and skills of its class, e.g. a
weapon. These plugins need a player in the game, so the admin panel doesn't run
them — add such items and skills on the edit page of the character if you need
them. Everything else, e.g. the level or the stats, can be edited there as well.

### Editing skills and the master skill tree

The **Learned Skills** of a character are split into two parts:

* The **regular skills** are listed with a **Remove** button each. **Add Skill**
  offers the skills which the character class can learn.
* The **master skill tree** looks like the one in the game: one column per master
  skill root and one row per rank. Enter the level of a master skill in its
  field — `0` removes it. A level is limited to the range the skill allows.

Below each master skill name you see the skills it requires (`↑`). Like in the
game, a master skill needs its required skills and a skill of the previous rank
at level 10 or higher. Skills whose requirements are not met yet are dimmed; a
skill which has a level although its requirements are not met gets a red dashed
border. The admin panel doesn't prevent this, so you can fix inconsistent data
in any order.

Changing a level by hand does not change the character's **Master Level Up
Points**. **Reset Master Skill Tree** removes all master skills after a
confirmation and adds the spent points (the sum of all master skill levels) back
to the master level up points, so the player can distribute them again.

All changes are stored when you save the page.

### Granting cash shop coins

**Coins** next to an account opens its [cash shop](../server-features/cash-shop.md)
coins: the balances of **WCoin (C)**, **WCoin (P)** and **Goblin Points**, and the
latest grants. Below the balances you grant coins to the account; a negative
amount takes coins again, but never below zero. It needs the administrator role.
API keys with the cash shop role grant coins through the
[API](authentication.md#granting-cash-shop-coins) instead.

A grant doesn't change the balance right away, it's *pending* until the game
server applies it — the next time the player opens the cash shop, even when the
player is online. That's why the balances can't be edited directly: the game
server keeps the account of an online player in memory and would overwrite such a
change.

### Changing a password

Passwords are stored as a hash, so they cannot be read back. To give a player a
new password, use the account edit page and set a new password there.

## Deleting accounts

Accounts can be deleted from the generic edit page. Consider banning instead —
a banned account keeps the character names reserved and keeps the history of what
happened on your server.
