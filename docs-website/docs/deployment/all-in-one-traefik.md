---
title: All-in-one with Traefik
sidebar_position: 3
description: Deploy OpenMU behind Traefik, so it can share a machine with your website.
---

# All-in-one with Traefik

This variant is recommended if you want to host on a small machine with a low
amount of players **and** want to host your MU Online website on the same
machine. Traefik acts as reverse proxy, so multiple websites can share ports 80
and 443.

All kinds of OpenMU subsystems (connect server, game server, login server, admin
panel, …) still run in one process, but the admin panel and other websites can be
routed separately.

## How routing works

You tell Traefik how to handle incoming requests by adding labels to a container:

```yaml
services:
  admin-panel:
    # ...
    labels:
      - "traefik.enable=true"
      - "traefik.docker.network=proxy"
      - "traefik.http.routers.adm.entrypoints=websecure"
      - "traefik.http.routers.adm.rule=Host(`admin.domain.com`)"

  muonline-website:
    # ...
    labels:
      - "traefik.enable=true"
      - "traefik.docker.network=proxy"
      - "traefik.http.routers.muonline.entrypoints=websecure"
      - "traefik.http.routers.muonline.rule=Host(`muonline.domain.com`)"
```

You can even add multiple domains and/or subdomains to one host label:

```yaml
- "traefik.http.routers.muonline.rule=Host(`www.domain1.com`,`domain1.com`,`sub.domain1.com`)"
```

## Deployment with docker compose

### Install git

See [the git install guide](https://github.com/git-guides/install-git).

### Clone the repository

```bash
git clone https://github.com/MUnique/OpenMU.git
cd OpenMU/deploy/all-in-one-traefik
```

### Create a docker network

The containers of different docker compose files need to communicate with each
other:

```bash
docker network create proxy
```

### Option A — for local testing

```bash
docker compose -f docker-compose.yml up -d
```

It's then available on your local computer through a loopback IP. The admin panel
URL is [http://admin.docker.localhost](http://admin.docker.localhost). You can change it in the
`docker-compose.yml` file.

### Option B — with HTTPS

If you want to share your server with the world, set up HTTPS — Traefik can
handle it for you. Otherwise, traffic from and to the admin panel is not
encrypted.

Copy `.env.example` to `.env` and edit it with the domain/subdomain of your admin
panel URL:

```bash
cp .env.example .env
```

Copy `data-traefik/acme.example.json` to `data-traefik/acme.json` and give it
permission 600:

```bash
cp data-traefik/acme.example.json data-traefik/acme.json
chmod 600 data-traefik/acme.json
```

Then run it:

```bash
docker compose -f docker-compose.prod.yml up -d
```

## Admin panel users

The admin panel authenticates its users itself, so there is no basic
authentication middleware in Traefik anymore — and no restart after a user
changed. Users are managed on the [Users page](../admin-panel/users.md); the
login and the optional second factor are described under
[Signing in](../admin-panel/authentication.md).

## What's next

The server is automatically started and initialized for Season 6.

Go to the admin panel — locally that is [http://admin.docker.localhost/](http://admin.docker.localhost/). Until
the first admin panel user exists, it is reachable without a login; create one
before the server is reachable from the internet, or configure a bootstrap user
beforehand (see [Signing in](../admin-panel/authentication.md)).

If you want to run another game version, use the
[Setup page](../admin-panel/setup.md).
