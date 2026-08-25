---
title: Admin panel users
sidebar_label: Users
sidebar_position: 13
description: Manage who is allowed to log into the admin panel.
---

# Admin panel users

**Navigation:** *Users* — route `/users`

These are the users which may log into the **admin panel** — they are not game
accounts. Game accounts are managed on the [Accounts page](accounts.md).

:::note[Only shown when available]
The menu entry only appears when the panel actually manages an authentication
backend. In the docker deployments this is the `.htpasswd` file of the reverse
proxy, which is mounted into the container. When you run the server from source,
there is no reverse proxy and therefore no user management — and no
authentication either.
:::

## What you can do

| Action | Effect |
|---|---|
| **Create user** | Adds a user with a password |
| **Change password** | Sets a new password for an existing user |
| **Delete** | Removes a user. The last remaining user cannot be deleted, so you can't lock yourself out. |

Passwords are written to the `.htpasswd` file as bcrypt hashes.

:::tip[Do not edit `.htpasswd` by hand]
Use this page. If you edit the file manually you risk breaking the format, and
the changes may not match what the proxy expects.
:::

## Default credentials

The docker deployments ship with one user:

* user name `admin`
* password `openmu`

**Change this password before your server is reachable from the internet.** The
default is public knowledge — it is written in the repository.

## Traefik: restart after changes

In the [all-in-one with Traefik](../deployment/all-in-one-traefik.md) deployment,
Traefik reads the `.htpasswd` file at startup. After adding or changing a user you
have to **restart the Traefik container** for it to take effect.

The nginx-based deployments pick the change up without a restart.

## Securing the panel

Basic authentication only protects the panel if the connection is encrypted —
otherwise the password travels in plain text with every request. Set up HTTPS:

* [All-in-one with nginx and certbot](../deployment/all-in-one.md#option-b--with-https)
* [All-in-one with Traefik](../deployment/all-in-one-traefik.md#option-b--with-https)

Further hardening which is worth the effort on a public server:

* Do not expose the admin panel port to the whole internet if you can reach it
  through a VPN or an SSH tunnel instead.
* Give each administrator their own user, so you can remove one without changing
  everybody's password.
* Remember that admin panel access means full access to your players' account
  data.
