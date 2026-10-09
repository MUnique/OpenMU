---
title: Kubernetes
sidebar_position: 5
description: Hosting the distributed deployment of OpenMU on Kubernetes with Dapr and a Helm chart.
---

# Kubernetes

:::warning[Experimental]
Like the [distributed deployment](distributed.md) it's based on, this is
experimental. It requires a good understanding of Kubernetes and of the
[distributed deployment](distributed.md).
:::

The Helm chart in [`deploy/kubernetes/openmu`](https://github.com/MUnique/OpenMU/tree/master/deploy/kubernetes/openmu)
deploys the same containers as the [docker compose file](distributed.md), but
lets Kubernetes and Dapr do what the compose file does by hand:

| Workload | Kind | Instances |
|---|---|---|
| Central server (connect servers, login, guild, friend, chat server) | StatefulSet | Exactly one |
| Admin panel | StatefulSet | One |
| Game servers | StatefulSet | `gameServer.replicas` |

All of them are StatefulSets, because the names of their pods have to be stable:
each pod gets its own queues in RabbitMQ, which are named after the pod. For the
central server, it also ensures that there are never two of them, not even
during an update — it keeps its state in memory.

The Dapr sidecars are injected by Dapr, the Dapr components (pub/sub with
RabbitMQ, state with Redis, secrets from Kubernetes secrets) are part of the
chart.

## Game servers

All game servers are one StatefulSet:

* **Pod N runs the game server with the id `gameServer.idOffset` + N.** It takes
  the id from the number at the end of its host name (see
  [`GS_ID`](distributed.md#gs_id)). A game server definition has to exist for
  each id, e.g. created by the setup.
* **They share one Dapr app id.** Nothing addresses a game server by its app id:
  the calls to game servers are published to all of them, and each one only
  handles what concerns it.
* **They listen on the same ports,** `gameServer.listenerPort` and the following
  ones, one per client version (see
  [`GS_LISTENER_PORT`](distributed.md#gs_listener_port)). The chat server,
  however, listens on the port which is configured in the database, so it has to
  match `centralServer.service.chatServerPort` — see
  [upgrading an existing installation](../reference/ports.md#upgrading-an-existing-installation)
  if your database was initialized by an older version.

The game clients connect to the game servers directly, so they must be reachable
from outside the cluster. With `gameServer.exposure: hostPort` (the default),
each game server listens on these ports of its node, and announces the IP of its
node to the clients (`gameServer.resolveIp: hostIP`). Because they all use the
same ports, Kubernetes schedules at most one game server per node. If the IP
which Kubernetes knows for a node isn't the one the clients can reach, set
`gameServer.resolveIp` to e.g. `public`.

On shutdown, a game server saves and disconnects its players. It gets
`gameServer.terminationGracePeriodSeconds` for that. Its Dapr sidecar stays up
until the game server is done, so that the players get logged off at the central
server.

## Requirements

* **Dapr** in the cluster, e.g. with its Helm chart:

  ```bash
  helm repo add dapr https://dapr.github.io/helm-charts
  helm install dapr dapr/dapr --namespace dapr-system --create-namespace --wait
  ```

* **The OpenMU images.** There are no up-to-date published images yet. Build
  them from the sources and push them to a registry your cluster can pull from:

  ```bash
  cd src
  docker build -f Dapr/CentralServer.Host/Dockerfile -t <registry>/munique/openmu-central:<tag> .
  docker build -f Dapr/GameServer.Host/Dockerfile -t <registry>/munique/openmu-game:<tag> .
  docker build -f Dapr/AdminPanel.Host/Dockerfile -t <registry>/munique/openmu-admin:<tag> .
  ```

* **PostgreSQL, RabbitMQ and Redis.** For development and tests, the chart
  deploys simple instances of them (`infrastructure.enabled`). For production,
  use your own and configure `database`, `rabbitmq` and `redis`.

## Installation

```bash
helm install openmu deploy/kubernetes/openmu --namespace openmu --create-namespace \
  --set image.registry=<registry> --set image.tag=<tag> \
  --set adminPanel.bootstrapUser.name=admin --set adminPanel.bootstrapUser.password=<password>
```

Then open the admin panel, e.g. with `kubectl port-forward`, or through the
ingress (`ingress.enabled`), which routes `/admin` to the admin panel and
`/serverInfo` to the central server. Install the database on the
[Setup page](../admin-panel/setup.md). The central and game servers become ready
afterwards — see the [health endpoints](distributed.md#health-endpoints).

The game clients connect to the connect servers through the service
`<release>-central-server`, which is a `LoadBalancer` by default
(`centralServer.service`).

See [`values.yaml`](https://github.com/MUnique/OpenMU/blob/master/deploy/kubernetes/openmu/values.yaml)
for all settings.

## Limitations

* **One release per namespace.** The connection strings are stored in the secret
  `connectionstrings`, whose name is fixed.
* **The game servers need a node each,** when they're exposed with `hostPort`.
  Other ways, e.g. one `LoadBalancer` service per game server, need an IP per
  game server which the game server knows, which isn't supported yet.
* **No live map** through the ingress: it's served by each game server under
  `/gameServer/<id>/`, which would need a route per pod.
* Observability: set `otlpEndpoint` to the OTLP endpoint of your collector.
