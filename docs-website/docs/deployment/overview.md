---
title: Overview
sidebar_position: 1
description: Choosing a deployment variant for your OpenMU server.
---

# Deployment overview

The recommended way to deploy OpenMU is through Docker. Depending on the scale
you need, there are multiple ways to do that.

| | [All-in-one](all-in-one.md) | [All-in-one with Traefik](all-in-one-traefik.md) | [Distributed](distributed.md) |
|---|---|---|---|
| Recommended for | Small machine, low player count | Same, plus other websites on the same machine | Large setups |
| Processes | One | One (admin panel separately routable) | Many containers |
| Reverse proxy | nginx | Traefik | nginx |
| HTTPS | certbot | Traefik / Let's Encrypt | certbot |
| Status | Supported | Supported | **Currently broken and unsupported** |

## All-in-one

The [all-in-one deployment](all-in-one.md) is recommended if you want to host on
a small machine with a low amount of players. All kinds of OpenMU subsystems
(connect server, game server, login server, admin panel, …) run in one process.

**Pros**

* No communication overhead between subsystems, therefore slightly faster
* Simpler deployment
* Smaller memory footprint — since everything runs in one process, there is no
  overhead of multiple processes and runtimes, and data can be shared
* Easier to observe and debug, no additional tools required

**Cons**

* Harder to scale — only by scaling up your single machine
* Lower resiliency: if one subsystem crashes the process, the whole thing goes down
* It's a more or less self-contained system which is harder to extend

## All-in-one with Traefik as reverse proxy

The [all-in-one with Traefik deployment](all-in-one-traefik.md) is recommended if
you want to host on a small machine with a low amount of players **and** want to
host your MU Online website on the same machine.

Once Traefik works as a reverse proxy, you can handle multiple websites without
changing the default port for HTTP/HTTPS connections. By adding a few labels to
your container, you tell Traefik how to handle incoming requests and it redirects
to the correct website.

**Pros** — the same as the all-in-one deployment, plus:

* You can have multiple websites with auto-renewed SSL certificates
* Only ports 80 and 443 have to be exposed for websites and the admin panel;
  Traefik knows what to do

**Cons** — the same as the all-in-one deployment.

## Distributed

:::danger[Currently broken and unsupported]
The docs of the distributed deployment are out of date and it is unsupported due
to several issues which have to be resolved first. Feel free to contribute — see
the [open issues with the `distributed-deployment` label](https://github.com/MUnique/OpenMU/issues?q=is%3Aissue%20state%3Aopen%20label%3Adistributed-deployment).
:::

It is also possible to host OpenMU in a [distributed](distributed.md) way. This
introduces a lot more complexity and you should know what you are doing. The
communication between the subsystems is handled with [Dapr](https://dapr.io/).

**Pros**

* Easier to scale. For example, if you need additional game servers, you simply
  add more containers.
* Higher resiliency: if one subsystem crashes, the others are not affected.
* It's easier to add more subsystems, even custom ones. For example, one could
  subscribe to already published events like guild messages or letters, and
  forward them to other systems (e-mail, Discord, …).

**Cons**

* Communication overhead between subsystems
* Higher memory footprint, since multiple docker containers run (each with their
  own .NET runtime) which can't share some data
* Harder to observe and debug. Loki, Grafana, Prometheus and Zipkin are included
  to compensate for that, but they require additional resources.
