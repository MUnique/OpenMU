---
title: Run with Docker
sidebar_position: 2
description: Start an OpenMU server on your machine with docker compose.
---

# Run with Docker

This is the fastest way to get a running server. All OpenMU subsystems
(connect server, game servers, chat server, login server, admin panel) run in one
container, next to a PostgreSQL container and an nginx container.

## Prerequisites

* [Docker](https://docs.docker.com/get-started/get-docker/) with the compose plugin
* [git](https://github.com/git-guides/install-git)

## Clone the repository

```bash
git clone https://github.com/MUnique/OpenMU.git
cd OpenMU/deploy/all-in-one
```

## Start it

To use the official docker image, run:

```bash
docker compose up -d --no-build
```

That's it — the server is available on your local machine through a loopback IP.

## Open the admin panel

Go to [http://localhost/](http://localhost/).

* On a fresh installation there is no user yet, so the panel lets you in
  without a login and says so.
* **Create your first user** right after the installation, or configure a
  bootstrap user before the first start — see
  [Signing in](../admin-panel/authentication.md).

The server is automatically initialized for Season 6, so you can start playing
right away. If you want another game version, another number of game servers, or
test accounts, use the [Setup page](../admin-panel/setup.md).

## Next steps

* [Connect a game client](game-client.md)
* [Test accounts](test-accounts.md) that exist in a freshly initialized database
* [Deployment](../deployment/overview.md) — HTTPS, domains and the other
  deployment variants, when you want other people to play on your server
* [Admin panel](../admin-panel/overview.md) — how to operate and configure the
  running server

## Database environment variables

The postgres connection strings of the container can be influenced with these
environment variables:

| Name | Description |
|---|---|
| `DB_HOST` | The host name of the database. If the local configuration file is still configured to use `localhost`, the value of this variable replaces it. |
| `DB_ADMIN_USER` | The user name of the postgres admin account. If the local configuration file is still configured to use `postgres` for the user name of the admin (first entry in the `ConnectionSettings.xml`), the value of this variable replaces it. |
| `DB_ADMIN_PW` | The password of the postgres admin account. If the local configuration file is still configured to use `admin` for the password of the admin (first entry in the `ConnectionSettings.xml`), the value of this variable replaces it. |

More variables and the start parameters are listed under
[Startup parameters and environment variables](../deployment/startup-parameters.md).
