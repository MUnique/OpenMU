---
title: Plugins
sidebar_position: 8
description: Activate, deactivate and configure the plugins which implement the game features.
---

# Plugins

**Navigation:** *Configuration → Plugins* — route `/plugins`

Large parts of OpenMU are implemented as plugins: packet handlers, view plugins,
chat commands, periodic tasks, item crafting, quest logic, and much more. Each
plugin implements an **extension point** (a plugin interface), and the server
picks the plugins which are active for the client version it is serving.

This page lists every plugin the server knows and lets you turn it on and off.

## The list

| Column | Meaning |
|---|---|
| Extension point | The interface this plugin implements. Hover for its description. |
| Plugin name / Help | The display name. Hover over the name or expand Help to read the description. |
| Plugin type | The .NET type. Hover for its type id (a GUID). |
| Action | Activate/Deactivate and, if the plugin has one, its configuration |

Above the list you can filter by extension point, by name or description, and by
type. The name search highlights matching words in the expanded help text.

## Activating and deactivating

**Deactivate** disables a plugin, **Activate** enables it again. This is how you

* switch off a feature you do not want on your server,
* replace a built-in behaviour by your own plugin: deactivate the original,
  activate your extended or modified version.

:::warning[Deactivate carefully]
Many plugins are not optional features but the implementation of a game
mechanic — a packet handler or a view plugin. Deactivating one can make the
client behave strangely or disconnect. If in doubt, note the type name before you
change anything, so you can turn it back on.
:::

## Plugin configuration

Plugins with their own configuration have a ⚙ button which opens it. The
configuration is stored per plugin and, where applicable, per client version, so
the same plugin can behave differently for different clients.

The custom plugin configuration can also contain a *custom plugin source*, which
is how a plugin can be adapted without rebuilding the server.

## Periodic event server scope

Periodic tasks — invasions, Happy Hour, Blood Castle, Devil Square and the
like — run on every game server by default, each with its own copy of the
event. Their configuration has an **Active on servers** list: leave it empty
to keep the default, or tick selected servers to run the event only there.
Restricting an open-world event to selected servers keeps it contested —
players can't clear it on one server and farm it again on another, and a
no-PvP server doesn't become a risk-free farm.

A few semantics worth knowing:

* A forced start (game master command or admin action) starts the event on the
  server where it was requested — every game server runs its own copy, so a
  force never crosses servers. A game master on an excluded server only gets a
  note that the event isn't available there; nothing starts.
* Removing a server from the list stops new runs there, but a running event
  always finishes and cleans up. Open mini-game lobbies stay joinable until
  they end.
* An explicit selection pins today's servers: servers added later don't run
  the event until ticked. Leave the list empty to include future servers.
  Make sure the event server has headroom (`MaximumPlayers`) for players
  coming over from other servers.

## When do changes take effect?

Activation state is stored in the configuration. Depending on the extension
point, the change is picked up by the running server or requires a reload — use
**Reload configuration and restart all game servers** on the
[Servers page](servers.md) if you don't see an effect.

## Feature plugins

Some plugins are not a single mechanic but a whole feature with its own
configuration, grouped in the *Feature Plugins* extension point. They are
documented separately:

* [Server-side AI bots](../server-features/bots.md)

## Game event publisher

The *Game Event Publisher* plugin publishes notable events of the game to the
whole server, so that external systems can react on them, for example a Discord
integration (see [Discord](../server-features/discord.md)). It's
disabled by default; activate it when such a system is connected.

It publishes:

* the opening, start and end of mini games, like Blood Castle, with the winner,
* the start and end of invasions, with the invaded maps,
* the state changes of the castle siege, with the owner of the castle,
* excellent and ancient items which are dropped by monsters,
* the kills of boss monsters,
* characters which reach a level or master level milestone,
* global notices which game masters send with the `!` chat prefix.

Each kind of event can be switched off in the plugin configuration. There you
also define which monsters are bosses, by their monster numbers (e.g. 275 for
Kundun), and which levels and master levels are milestones (by default, level
400).

## Discord integration of the client

The feature plugin *Discord integration of the client* tells game clients which
ask for it how the server is connected to Discord: the invite link, the Discord
application of the Rich Presence, the link of the account and which chats are
mirrored. It also lets players link their account from the client, and sends
them the messages written in Discord as such. See
[Discord](../server-features/discord.md#the-game-client).

## Server announcements

The *Server announcements* plugin shows a list of announcements to all players,
one after another, so that a game master doesn't have to type recurring ones
like rule reminders or a link to the community. It's disabled by default.

Its configuration has:

* the *Interval* between one announcement and the next (at least 10 seconds),
* the *Announcements*, each with its text and its message type, e.g. the golden
  message in the centre of the screen or a blue system message.

The announcements are shown in their order; after the last one, it starts over
with the first one. Each game server has its own rotation. The text can be translated:
players see the translation for their language, if there is one.

## Writing your own plugin

Implementing a plugin is documented next to the code, in the
[MUnique.OpenMU.PlugIns readme](https://github.com/MUnique/OpenMU/blob/master/src/PlugIns/Readme.md).
