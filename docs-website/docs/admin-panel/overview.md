---
title: Overview
sidebar_position: 1
description: What the OpenMU admin panel is, how to open it and how it is structured.
---

# The admin panel

The admin panel is the user interface of the server. It is used to operate the
running server (start and stop listeners, watch and disconnect players, read
logs) and to configure the game itself (items, monsters, maps, drops, rates,
plugins, chat commands).

It is implemented with ASP.NET Core Blazor Server and hosted by the server
process itself on an embedded Kestrel web server — there is no separate
installation.

## Opening the panel

| Deployment | URL |
|---|---|
| [From source](../getting-started/from-source.md) | [http://localhost/](http://localhost/) |
| [All-in-one docker](../deployment/all-in-one.md) | [http://localhost/](http://localhost/) |
| [All-in-one with Traefik](../deployment/all-in-one-traefik.md) | [http://admin.docker.localhost/](http://admin.docker.localhost/) (locally) |
| [Distributed](../deployment/distributed.md) | [http://localhost/admin](http://localhost/admin) |

The panel asks for a login of its own, in every deployment. See
[Signing in](authentication.md) for the login, the optional second factor and
the roles.

:::danger[Secure the panel before exposing it]
Whoever reaches the admin panel controls your server and can read and modify all
account data. Before your server is reachable from the internet:

* create your first user on the [Users page](users.md), so the panel leaves
  its initial setup mode in which it is reachable without a login,
* set up HTTPS, so the session is not sent in plain text
  ([all-in-one](../deployment/all-in-one.md#option-b--with-https),
  [Traefik](../deployment/all-in-one-traefik.md#option-b--with-https)).

When you run the server from source, there is no reverse proxy and therefore
**no authentication at all** — keep that setup on your local machine, or start it
with `-adminpanel:disabled`.
:::

## What you find where

| Page | Purpose |
|---|---|
| [Setup](setup.md) | Create, reinstall and update the database; choose the game version |
| [Configuration updates](configuration-updates.md) | Apply configuration data updates which come with new OpenMU versions |
| [Servers](servers.md) | Start/stop listeners, player counts, add servers, global messages |
| [Accounts](accounts.md) | Search, create, ban and edit accounts |
| [Online accounts](online-accounts.md) | Who is logged in, disconnect players, offline sessions |
| [Game configuration](game-configuration.md) | Everything about the game world: system settings, items, monsters, skills, maps, drops, … |
| [Plugins](plugins.md) | Activate, deactivate and configure plugins |
| [Chat commands](chat-commands.md) | Which in-game commands exist and who may use them |
| [Map editor](map-editor.md) | Edit monster spawn areas and gates graphically |
| [Live map](live-map.md) | Watch what happens on a map in real time |
| [Logs and monitoring](logs-and-monitoring.md) | Log files, Grafana, Prometheus, Zipkin |
| [Users](users.md) | The users which may log into the admin panel |
| [API keys](authentication.md#api-keys-for-external-applications) | The keys with which external applications use the public API |

Server features which are configured through the panel have their own pages, for
example the [server-side AI bots](../server-features/bots.md).

## Layout

* **Navigation menu** (left) — the pages listed above. The *Configuration*
  entry is a drop-down with the game configuration pages.
* **Breadcrumb** (top) — the generic edit pages can nest deeply into the
  configuration; the breadcrumb is how you get back out.
* **Configuration search** — a search box which finds configuration objects by
  name and jumps directly to their edit page.
* **Theme selector** — light and dark theme.
* **Language selector** — the panel's texts come from resource files. English is
  included; further languages can be added as satellite resource files and appear
  in this selector automatically.

## A note about the edit pages

To be able to edit most of the data without writing SQL, there are **generic edit
pages** which are generated automatically by reflection from the data model.

Keep in mind that these pages are a very technical and generic view of the data,
so you need to know what you are doing. Some fields can't be edited or created
yet, because not every type has a corresponding component. More user-friendly
editors — like the [map editor](map-editor.md) and the item editor — are added
step by step.

See [Game configuration](game-configuration.md#the-generic-edit-pages) for how
they work.

## Differences between the deployments

Some parts of the panel depend on how the server is hosted:

| | All-in-one | Distributed |
|---|---|---|
| Logs | [Log files page](logs-and-monitoring.md#log-files-all-in-one) inside the panel | Links to Grafana/Loki, metrics and Zipkin |
| Live map | Rendered by the panel itself | Reverse-proxied from the game server container |
| Auto start / auto schema update | Applies | Ignored — listeners always start, schema updates are started manually |
| After an installation | Ready immediately | The connect server and game server containers have to be restarted |

## Planned

The panel grows with the server. Ideas which are not implemented yet:

* More user-friendly configuration editors, and account/character editors which
  know the game rules instead of showing the raw data model
* Expanding a server in the list to see the players who are playing on it
* More game master functions on the [live map](live-map.md)
