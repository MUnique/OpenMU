---
title: Distributed
sidebar_position: 4
description: Hosting OpenMU as multiple containers which communicate through Dapr.
---

# Distributed deployment

:::danger[Currently broken and unsupported]
This way of hosting OpenMU is currently unsupported due to several issues which
have to be resolved first, and the documentation is out of date. Feel free to
contribute — see the
[open issues with the `distributed-deployment` label](https://github.com/MUnique/OpenMU/issues?q=is%3Aissue%20state%3Aopen%20label%3Adistributed-deployment).

It also requires a good understanding of distributed systems and more resources
(CPU, RAM, disk, network) than the [all-in-one deployment](all-in-one.md).
:::

Each subsystem runs in its own container and the communication between them is
handled with [Dapr](https://dapr.io/). Loki, Grafana, Prometheus and Zipkin are
included for observability.

## Deployment with docker compose

Currently there is only a docker compose file for the deployment, which has the
limitation that everything runs on the same physical machine. For a truly
distributed environment with multiple machines, Kubernetes can be used — but
there is no finished Kubernetes configuration yet. Contributions are welcome.

### Clone the repository and navigate to the compose files

```bash
git clone https://github.com/MUnique/OpenMU.git
cd OpenMU/deploy/distributed
```

### Option A — for local testing

```bash
docker compose up -d --no-build
```

It's then available on your local computer through a loopback IP.

### Option B — with HTTPS

Set the environment variable `DOMAIN_NAME` for docker compose. This can be done
in
[various ways](https://docs.docker.com/compose/environment-variables/set-environment-variables/),
e.g. by editing `docker-compose.prod.yml` or setting it in your shell. The
variable is replaced in the nginx template config files.

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

Run certbot explicitly (replace `example.org` with your domain):

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml \
  run --rm certbot certonly --webroot --webroot-path /var/www/certbot/ -d example.org
```

Certificates expire after 3 months, so renew them regularly — ideally with a cron
job:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml run --rm certbot renew
```

## What's next

Discover the [admin panel](../admin-panel/overview.md). If your containers run on
docker at your local machine, go to [http://localhost/admin](http://localhost/admin). Until the first
admin panel user exists, the panel is reachable without a login — create one on
the [Users page](../admin-panel/users.md), or configure a bootstrap user before
the first start (see [Signing in](../admin-panel/authentication.md)).

On the [Setup page](../admin-panel/setup.md) you select the game version, the
number of game servers (just the data of it), and whether test accounts should be
created. Click *Install* and wait until the database is set up and filled with
the data.

:::note[Restart the containers after an installation]
In a distributed deployment, the connect server and game server containers have
to be restarted after the installation finished. The admin panel tells you so
when it is done.
:::

## Differences to the all-in-one deployment

Some functions of the admin panel behave differently, because the panel runs in
its own process:

* **Logs and metrics** are not read from local log files. Instead the navigation
  menu links to Grafana (Loki), the metric dashboards and Zipkin — see
  [Logs and monitoring](../admin-panel/logs-and-monitoring.md).
* **Live map** links point to the reverse-proxied map application of the
  respective game server container.
* **Auto start** and **auto update schema** of the
  [System configuration](../admin-panel/game-configuration.md#system) only apply
  to the all-in-one startup. The distributed processes always start their
  listeners automatically, and the schema update has to be started manually over
  the admin panel.

## Environment variables

The OpenMU images used in this docker compose consider the following environment
variables.

### `ASPNETCORE_ENVIRONMENT`

Usually specified correctly in the docker compose files. It has an effect on the
IP resolver, see below.

### `RESOLVE_IP`

Similar to the `-resolveIP` start parameter of the all-in-one startup project.
The defaults usually work fine, so you should try not to set this variable.

| Value | Description |
|---|---|
| `local` | Default in a *Development* environment. Determines a local IP; if none is found, a loopback IP is used (`127.127.127.127`). |
| `public` | Default in a *Production* environment. The public IP is determined by an [external API](https://www.ipify.org/). |
| `loopback` | Returns `127.127.127.127`, useful only if server and client run on the same machine. |
| *custom IP* | A custom IP, e.g. `192.168.0.1`. |

### `GS_ID`

Usually specified correctly in the docker compose files for each game server. It
specifies the id of a game server and is used to retrieve the
`GameServerConfiguration` from the database.

See [Startup parameters and environment variables](startup-parameters.md) for the
variables which apply to every deployment.
