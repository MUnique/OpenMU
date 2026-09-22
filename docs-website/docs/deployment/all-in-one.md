---
title: All-in-one
sidebar_position: 2
description: Deploy OpenMU with docker compose, nginx and certbot.
---

# All-in-one deployment

All kinds of OpenMU subsystems (connect server, game server, login server, admin
panel, …) run in one process, next to a PostgreSQL container and an nginx
container which acts as reverse proxy.

## Install git

See [the git install guide](https://github.com/git-guides/install-git).

## Clone the repository

```bash
git clone https://github.com/MUnique/OpenMU.git
```

It creates a new folder `OpenMU` with the repository contents inside.

## Navigate to the docker compose files

```bash
cd OpenMU/deploy/all-in-one
```

## Option A — for local testing

To use the official docker image, just run:

```bash
docker compose up -d --no-build
```

That's it. It's then available on your local computer through a loopback IP.

If you want to make it available through the internet, choose option B.

## Option B — with HTTPS

If you want to share your server with the world, it's recommended to set up HTTPS
for nginx. Otherwise, traffic from and to the admin panel is not encrypted — and
that traffic includes the admin panel password.

### Set your domain name as environment variable

Specify the environment variable `DOMAIN_NAME` for docker compose. This can be
done in
[various ways](https://docs.docker.com/compose/environment-variables/set-environment-variables/),
e.g. by editing `docker-compose.prod.yml` or by
[setting it in your shell](https://phoenixnap.com/kb/linux-set-environment-variable).

This variable is replaced in the nginx template config files which get included
in the other configuration files.

### Run it

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Run certbot explicitly

Replace `example.org` with your domain:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml \
  run --rm certbot certonly --webroot --webroot-path /var/www/certbot/ -d example.org
```

### Set up certificate renewal

Certificates expire after 3 months, so it's recommended to renew them regularly:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml run --rm certbot renew
```

It makes sense to add a cron job (e.g. once a week) on your host machine for that.

## What's next

The server is automatically started and initialized for Season 6. You can start
playing right away.

Additionally, take a look at the [admin panel](../admin-panel/overview.md). If
your containers run on docker at your local machine, you can simply go to
[http://localhost/](http://localhost/).

:::danger[Create a user before you expose the server]
Until the first admin panel user exists, the panel is reachable without a login.
Create one on the [Users page](../admin-panel/users.md), or configure a bootstrap
user with `OPENMU_ADMIN_USER` and `OPENMU_ADMIN_PASSWORD` before the first start
— see [Signing in](../admin-panel/authentication.md).
:::

If you want to run another game version, go to the
[Setup page](../admin-panel/setup.md) through the navigation menu, where you can
select your desired game version, the number of game servers (just the data of
it), and whether test accounts should be created. Click *Install*, wait a bit
until the database is set up and filled with the data, and OpenMU is ready to
use.
