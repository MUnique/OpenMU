---
title: Chat commands
sidebar_position: 9
description: The in-game commands, their parameters and who is allowed to use them.
---

# Chat commands

**Navigation:** *Configuration → Chat commands* — route `/chat-commands`

Chat commands are the commands players and game masters type into the in-game
chat, e.g. `/post` or `/trade`. Each of them is a plugin, and this page is both
their documentation and their switchboard.

## The list

| Column | Meaning |
|---|---|
| Command | The command itself, e.g. `/post` |
| Description | Its name and what it does |
| Usage | The usage string, plus an expandable table of the parameters |
| Minimum character status | Whether a `Normal` player may use it, or only a `GameMaster` |
| Action | Activate/Deactivate and, if available, the command's configuration |

The parameter table of a command shows, for each parameter, its name, short name,
type, whether it is required, and the valid values it accepts.

Above the list you can filter by command text and by minimum character status —
filtering by `GameMaster` gives you the list of admin commands your game masters
have available.

## Activating and deactivating

Use **Deactivate** to remove a command from your server, and **Activate** to
bring it back. This is the place to switch off a command which does not fit your
server's rules.

## Who is a game master?

A command with the minimum character status `GameMaster` only works for accounts
whose state is `GameMaster` or `GameMasterInvisible`. You set that state on the
[Accounts page](accounts.md).

On a freshly initialized Season 6 database with test accounts, `testgm` and
`testgm2` are game master accounts — see
[Test accounts](../getting-started/test-accounts.md).

## Configuration

Some commands have their own configuration behind the ⚙ button, for example to
change limits or messages. It is the same mechanism as for the other
[plugins](plugins.md).
