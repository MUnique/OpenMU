---
title: Distributed
sidebar_position: 4
description: Hosting OpenMU as multiple containers which communicate through Dapr.
---

# Distributed deployment

:::warning[Experimental]
This way of hosting OpenMU works, but it's experimental and has some
[known limitations](#known-limitations). Feel free to contribute — see the
[open issues with the `distributed-deployment` label](https://github.com/MUnique/OpenMU/issues?q=is%3Aissue%20state%3Aopen%20label%3Adistributed-deployment).

It also requires a good understanding of distributed systems and more resources
(CPU, RAM, disk, network) than the [all-in-one deployment](all-in-one.md).
:::

The subsystems run in their own containers and the communication between them is
handled with [Dapr](https://dapr.io/):

| Container | Subsystems | Instances |
|---|---|---|
| `centralServer` | Connect servers (one per client version), login server, guild server, friend server, chat server | One |
| `gameServer0`, `gameServer1`, … | One game server each | One per game server |
| `adminPanel` | Admin panel | One |

The subsystems of the central server exist just once per deployment and keep
their state in memory, so they run together in one process.

Each of these containers has a Dapr sidecar in its own container, e.g.
`gameServer0-dapr`. A subsystem reaches its sidecar through the
`DAPR_HTTP_ENDPOINT` and `DAPR_GRPC_ENDPOINT` environment variables, and the
sidecar reaches the subsystem by its container name. So either one can be
restarted without affecting the other. All containers have a restart policy,
which starts them again when they crash, or when a game server ends itself to
apply a changed configuration.

Nothing addresses a game server by its Dapr app id: the calls to game servers,
e.g. a friend request or a global message from the admin panel, are published to
all game servers. The game server which hosts the affected player handles it, or
the one whose id is in the message, and the others ignore it. So all game
servers share the app id `game-server`. Each process still needs its own queues
in RabbitMQ to receive every message; they are named after the `POD_NAME` of its
sidecar, which therefore has to be unique.

For observability, all subsystems and their Dapr sidecars send logs, metrics and traces with
[OpenTelemetry](https://opentelemetry.io/) to the `otel-lgtm` container, which
bundles an OpenTelemetry collector, Loki, Prometheus, Tempo and Grafana.

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
docker compose up -d --build
```

There are no up-to-date images of the distributed deployment published yet, so
`--build` builds them from the sources of the cloned repository. The first build
takes a while.

It's then available on your local computer through a loopback IP.

### Option B — with HTTPS

Set the environment variable `DOMAIN_NAME` for docker compose. This can be done
in
[various ways](https://docs.docker.com/compose/environment-variables/set-environment-variables/),
e.g. by editing `docker-compose.prod.yml` or setting it in your shell. The
variable is replaced in the nginx template config files.

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
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

:::note[Starting after an installation]
Until the database is installed, the central server container fails to start and
gets restarted by Docker, and the game servers wait for their configuration. So
a short while after the installation finished, all of them should be running.
The admin panel tells you to restart the central server and game server
containers — that's only required when one of them doesn't come up:

```bash
docker compose restart centralServer gameServer0 gameServer1
```
:::

## Differences to the all-in-one deployment

Some functions of the admin panel behave differently, because the panel runs in
its own process:

* **Logs, metrics and traces** are not read from local log files. Instead the
  navigation menu links to Grafana — see
  [Logs and monitoring](../admin-panel/logs-and-monitoring.md).
* **Live map** links point to the reverse-proxied map application of the
  respective game server container.
* **Reload configuration and restart all game servers** on the
  [Servers page](../admin-panel/servers.md) ends the game server processes.
  Docker starts them again, because of the restart policy of their containers.
* **Auto start** and **auto update schema** of the
  [System configuration](../admin-panel/game-configuration.md#system) only apply
  to the all-in-one startup. The distributed processes always start their
  listeners automatically, and the schema update has to be started manually over
  the admin panel.

## Health endpoints

Every OpenMU container answers on port 8080:

* `/health/live` — healthy as long as the process runs. Use it for liveness
  probes, which restart a process. It doesn't depend on the database, because a
  restart wouldn't fix that.
* `/health/ready` — whether the process can do its work, with the result of each
  check as JSON. The central and game servers are not ready until the database is
  installed and up to date. A server which is stopped, e.g. in the admin panel,
  is reported as *degraded*, which still counts as ready. The admin panel is
  always ready, because the database gets installed through it.

The compose file uses `/health/ready` for the health status of the containers,
which `docker compose ps` shows.

## Known limitations

* Each game server is a separate service in the compose file, with its own id
  (`GS_ID`). To add a game server, create it in the admin panel, and copy a game
  server service and its sidecar service in the compose file. Give them the next
  id, their own names, `POD_NAME` and ports.
* The compose file publishes the ports 55901–55902 of `gameServer0` and
  55903–55904 of `gameServer1`. That matches the endpoints which the setup
  creates when there are two client versions. With one client version, the setup
  assigns 55901 to server 0 and 55902 to server 1, so adjust the published ports
  to the endpoints of the game servers in the admin panel.
* Everything runs on the same machine, see above.

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

When it's not set, the id is the number at the end of the host name, e.g. `3` for
`gameserver-3`. That's the name of a pod of a Kubernetes StatefulSet, so the game
servers can be one StatefulSet with a number of replicas. To use a range which
doesn't start at 0, add an offset with `GS_ID_OFFSET`. Without both, the id is
`0`.

### `GS_LISTENER_PORT`

Optional. By default, a game server listens on the ports which are configured for
its endpoints in the admin panel, e.g. 55901 and 55902 for server 0 with two
client versions, and 55903 and 55904 for server 1. When `GS_LISTENER_PORT` is
set, the game server listens on this port for its first endpoint, on the next
port for the second, and so on (in the order of the configured ports) — the same
ports in every container. That's useful when all game servers share one
template, like the pods of a Kubernetes StatefulSet.

The game server then also announces these ports to the clients, unless an
*alternative published port* is configured for the endpoint. So each game server
needs its own public IP (see `RESOLVE_IP`), or its own alternative published
ports when they share one IP.

### `DAPR_HTTP_ENDPOINT` and `DAPR_GRPC_ENDPOINT`

Usually specified correctly in the docker compose files. They specify the
address of the Dapr sidecar of a subsystem, e.g. `http://gameServer0-dapr:3500`
and `http://gameServer0-dapr:50001`. Without them, the subsystem expects its
sidecar on `localhost`.

### `OTEL_EXPORTER_OTLP_ENDPOINT`

The address of the OpenTelemetry (OTLP) endpoint to which a subsystem sends its
logs, metrics and traces, e.g. `http://otel-lgtm:4317`. Without it, nothing is
exported. The other standard
[OpenTelemetry variables](https://opentelemetry.io/docs/specs/otel/configuration/sdk-environment-variables/)
work as well, e.g. `OTEL_EXPORTER_OTLP_PROTOCOL` or `OTEL_RESOURCE_ATTRIBUTES`.

The log levels follow the usual .NET configuration. By default, everything from
`Debug` is exported and everything from `Information` is written to the console,
except the framework logs (`Microsoft.*`), which start at `Warning`. To change
it, set for example `Logging__LogLevel__Default` or
`Logging__LogLevel__MUnique.OpenMU.GameLogic`.

See [Startup parameters and environment variables](startup-parameters.md) for the
variables which apply to every deployment.

## Observability backend

The `otel-lgtm` container ([grafana/otel-lgtm](https://github.com/grafana/docker-otel-lgtm))
is meant for development, demo and test environments. It keeps its data in the
`otel-lgtm-data` volume. For a bigger production setup, run an OpenTelemetry
collector, Loki, Prometheus, Tempo and Grafana as separate services, or use a
hosted service, and point `OTEL_EXPORTER_OTLP_ENDPOINT` of the OpenMU services
and the tracing endpoint in `dapr-components/config.yaml` to it.
