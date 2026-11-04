---
title: Admin panel users
sidebar_label: Users
sidebar_position: 14
description: Manage who is allowed to log into the admin panel.
---

# Admin panel users

**Navigation:** *Users* — route `/users`

These are the users which may log into the **admin panel** — they are not game
accounts. Game accounts are managed on the [Accounts page](accounts.md).

The page requires the *Administrator* role. How the login itself works, and how
each user protects its own account with a second factor, is described under
[Signing in](authentication.md).

:::note[Admin users are stored separately from game accounts]
They live in an own `admin` schema of the database, not in the account table of
the game. A game password is typed into the game client and travels over the
game protocol, while an admin panel user can restart servers, edit the whole
configuration and read the logs — those two should not be the same secret. The
schema is also not readable by the game servers.
:::

## What you can do

| Action | Effect |
|---|---|
| **Create user** | Adds a user with a password and a role |
| **Change password** | Sets a new password for an existing user |
| **Role** | Changes the role. Running sessions of that user end, so the new role takes effect immediately. |
| **Reset second factor** | Removes the authenticator of a user which lost its app and its recovery codes. The user can set a new one up afterwards. |
| **Delete** | Removes a user. The last remaining user cannot be deleted, so you can't lock yourself out. |

Passwords are stored as bcrypt hashes and have to be at least 12 characters
long.

A configured [bootstrap user](authentication.md#bootstrap-user) is defined by the
environment and can't be edited here.

## Creating the first user

On a fresh installation the panel has no user yet and is therefore reachable
without a login. Create your first user right after the
[installation](setup.md) finished — or, better, configure a
[bootstrap user](authentication.md#bootstrap-user) before the first start, so
that window never exists.

## Securing the panel

Whoever reaches the admin panel controls your server and can read and modify all
account data. Before your server is reachable from the internet:

* make sure a real user exists, so the panel is not in its initial setup mode,
* set up HTTPS, so the session is not sent in plain text
  ([all-in-one](../deployment/all-in-one.md#option-b--with-https),
  [Traefik](../deployment/all-in-one-traefik.md#option-b--with-https)),
* require a [second factor](authentication.md#two-factor-authentication) from
  everybody who can reach it.
