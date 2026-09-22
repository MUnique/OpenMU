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

In particular, **creating characters here is not supported** — character creation
involves initialization logic which the web interface does not run yet. Create
characters in the game client.
:::

### Changing a password

Passwords are stored as a hash, so they cannot be read back. To give a player a
new password, use the account edit page and set a new password there.

## Deleting accounts

Accounts can be deleted from the generic edit page. Consider banning instead —
a banned account keeps the character names reserved and keeps the history of what
happened on your server.
