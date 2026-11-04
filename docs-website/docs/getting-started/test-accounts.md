---
title: Test accounts
sidebar_position: 5
description: The accounts which are created automatically when the database is initialized.
---

# Test accounts

To test some features of the server, test accounts are created automatically when
the database is initialized — if you kept the *test accounts* option enabled on
the [Setup page](../admin-panel/setup.md).

The **password of each of these accounts is the same as the user name**.

## All game versions

| User name | Description |
|---|---|
| `test0` … `test9` | General test accounts, level 1 to 90 in 10 level steps |

## Season 6 only

| User name | Description |
|---|---|
| `test300` | General test account with level 300 |
| `test400` | General test account with level 400, master characters |
| `testgm` | Test account of a game master |
| `testgm2` | Test account of a game master with summoner and rage fighter characters |
| `testunlock` | Test account without characters, but with unlocked character classes |
| `quest1` | Test account for the level 150 quests |
| `quest2` | Test account for the level 220 quests |
| `quest3` | Test account for the level 400 quests |
| `ancient` | Test account with ancient item sets, level 330 characters |
| `socket` | Test account with socket item sets, level 380 characters |

:::danger[Do not use these on a public server]
These accounts have well-known credentials and some of them have game master
rights. If you initialize a public server with test accounts, delete or ban them
on the [Accounts page](../admin-panel/accounts.md) before players can reach it —
or initialize the database without test accounts in the first place.
:::
