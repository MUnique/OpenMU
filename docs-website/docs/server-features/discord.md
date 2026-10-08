---
title: Discord
sidebar_label: Discord
sidebar_position: 5
description: A Discord bot and notifications, which bring the game to your Discord server.
---

# Discord

OpenMU can bring your game into your Discord server, in two ways which can be
combined:

* **Notifications:** what happens in the game, like mini games, the castle siege
  or rare drops, is posted into your channels. This works with simple *webhooks*,
  or through the bot.
* **The bot:** it shows how many players are online, keeps a status message of
  the game servers up to date, answers slash commands like `/who`, and sets up
  the channels and roles of your Discord server with `/openmu setup`.

## Notifications

| Category | What is posted |
|---|---|
| `Events` | Mini games which open (e.g. Blood Castle), winners of mini games, invasions which start and end |
| `CastleSiege` | Guild registration, the start and the end of the battle, and the new owner of the castle |
| `WorldNews` | Boss kills, excellent and ancient items which monsters dropped, characters which reach a level milestone |
| `Notices` | Global notices which game masters send with the `!` chat prefix |

Each category is posted into its own channel. A category without channel isn't
posted.

For the notifications, the *Game Event Publisher* plugin has to be active
([Plugins](../admin-panel/plugins.md#game-event-publisher)). It's disabled by
default. In its configuration, you choose which events are published, which
monsters are bosses and which levels are milestones.

### With webhooks

Webhooks are the simplest way, because you don't need a bot:

1. In Discord, open the settings of a channel, go to *Integrations → Webhooks*,
   create a webhook and copy its URL. Do this for each channel which should
   receive messages.
2. Configure the URLs, e.g. as environment variables:

   ```yaml
   environment:
     - Discord__Webhooks__Events=https://discord.com/api/webhooks/...
     - Discord__Webhooks__CastleSiege=https://discord.com/api/webhooks/...
     - Discord__Webhooks__WorldNews=https://discord.com/api/webhooks/...
     - Discord__Webhooks__Notices=https://discord.com/api/webhooks/...
   ```

Treat the URLs like passwords: everybody who knows one can post into the channel.
That's why they are not part of the game configuration in the database, but of
the configuration of the server process.

### With the bot

When the bot runs, it posts the notifications into the channels of the
[layout](#setting-up-your-discord-server), e.g. the `Events` into `#events`. It
finds them by their name, so you don't need to configure anything.

You can also choose other channels by their identifiers, e.g.
`Discord__Bot__Channels__Events=123456789012345678`. To copy the identifier of a
channel, enable the *Developer Mode* in the advanced settings of Discord, then
right-click the channel and select *Copy Channel ID*.

A category with a channel of the bot is posted by the bot, otherwise by its
webhook.

## The bot

### Creating the bot

1. Open the [Discord developer portal](https://discord.com/developers/applications),
   create a *New Application* and give it the name of your server.
2. Under *Bot*, click *Reset Token* and copy the token. Treat it like a password.
   The bot doesn't need any of the *Privileged Gateway Intents*.
3. Under *OAuth2 → URL Generator*, select the scopes `bot` and
   `applications.commands`, and the permissions *View Channels*, *Send Messages*,
   *Embed Links* and *Read Message History*. For [`/openmu setup`](#setting-up-your-discord-server),
   also select *Manage Channels* and *Manage Roles*. Open the generated URL to
   invite the bot to your Discord server.
4. Configure the token, e.g. as environment variable `Discord__Bot__Token`.
5. Restart the server. In the all-in-one deployment, the bot appears in the
   server list of the admin panel.

In the distributed deployment, the bot runs in its own container, see
[Distributed deployment](../deployment/distributed.md#discord).

### Slash commands

| Command | Shows |
|---|---|
| `/online` | The online players of each game server, and in total |
| `/who <character>` | The class, the levels and the online state of a character. Characters which are invisible in the game appear offline. |
| `/guild <name>` | The guild master, the number of members and the members which are online |
| `/events` | The events of the next 24 hours, like mini games and invasions |
| `/rank` | The ten best characters by resets, master level and level |

The commands only show information; they don't change anything in the game.
Additionally, administrators of the Discord server can use
[`/openmu setup`](#setting-up-your-discord-server).

When you configure the identifier of your Discord server (`Discord__Bot__GuildId`),
the commands are available immediately. Otherwise, Discord may take up to an hour
to show them.

### Status

The presence of the bot shows how many players are online. In the status channel
of the layout (`#server-status`), or in the one which you configure
(`Discord__Bot__StatusChannelId`), the bot keeps a message with the status of the
game servers up to date. Use a channel in which only the bot writes; it finds its
message again after a restart.

### Setting up your Discord server

The bot can set up your Discord server: run `/openmu setup` as administrator of
the Discord server. It creates the following roles, categories and channels:

| Category | Channels |
|---|---|
| Info | `#announcements` (notices of game masters), `#events` (events and castle siege), `#server-status` |
| Community | `#world-news`, `#world-chat`, `#general` |
| Staff | `#staff-alerts`, `#gm-commands` — only visible to the role *GM* |

The roles are *GM* and *Linked Player*. Only the bot can write into the
channels for notifications and the status. `#announcements` becomes an
*Announcement channel* on community servers, so that other Discord servers,
e.g. the ones of guilds, can follow it.

You can run the command again at any time: what already exists is adopted by
its name and nothing is created twice. Nothing is deleted or renamed, and only
the permissions which the layout defines are changed. So you can also start
with an existing Discord server, or with a server template (see below), and
let the command add what's missing. The answer of the command shows what was
created, what already existed and what failed.

The bot finds the channels by their name: when you rename one, configure its
identifier instead, as described above. When the bot is in more than one
Discord server, it posts into the channels of the one which you configure as
`Discord__Bot__GuildId`.

#### Your own layout

The layout is defined in a JSON file. To use your own, copy the
[default layout](https://github.com/MUnique/OpenMU/blob/master/src/Discord/Provisioning/DefaultLayout.json),
adapt it and configure its path as `Discord__Bot__LayoutFile`. Each role,
category and channel has a key, which has to be unique, and a name. Channels can
have these properties:

| Property | Description |
|---|---|
| `topic` | The topic of the channel. |
| `isAnnouncement` | Creates an announcement channel, on community servers. |
| `isReadOnly` | Only the bot can write into the channel. |
| `notifications` | The categories of notifications which are posted into the channel, e.g. `[ "Events", "CastleSiege" ]`. |
| `showsStatus` | The bot shows the status of the game servers in the channel. |

With `visibleTo`, a category is only visible to the listed roles, e.g.
`"visibleTo": [ "gm" ]`.

#### Server templates

Discord can copy the structure of a Discord server with a *server template*:
In the settings of a set-up Discord server, open *Server Template*, create a
template and share its link. Everybody who opens the link creates a new Discord
server with the same channels, roles and permissions, but without messages and
members. After inviting the bot to it, `/openmu setup` adopts everything and
reports that nothing was missing.

## Settings

All settings are part of the `Discord` section of the configuration, so they can
also be set in the `appsettings.json` of the server. As environment variables,
the parts are separated by two underscores.

| Setting | Default | Description |
|---|---|---|
| `Webhooks__<Category>` | — | The webhook URL per category, see the table above. |
| `Language` | `en` | The language of the messages and the bot. English (`en`) and German (`de`) are available. Names of maps, monsters and items are translated, too. |
| `EventAnnouncingServerIds__0`, `__1`, … | all | The identifiers of the game servers which announce mini games and invasions. When several game servers run the same events, set it to one of them to avoid duplicate messages. |
| `MaximumQueuedMessages` | `100` | The number of messages per channel which can wait to be sent. When there are more, the oldest ones are dropped. |
| `Bot__Token` | — | The token of the bot. Without it, the bot doesn't run. |
| `Bot__GuildId` | — | The identifier of your Discord server, for the slash commands and the channels of the layout. |
| `Bot__StatusChannelId` | the layout | The identifier of the channel with the status message. |
| `Bot__Channels__<Category>` | the layout | The identifier of the channel per category, into which the bot posts the notifications. |
| `Bot__LayoutFile` | the default | The path of a JSON file with your own [layout](#your-own-layout). |
| `Bot__StatusUpdateInterval` | `00:01:00` | How often the status message and the presence are updated. |

## Good to know

* **Times** like "the entrance closes in 5 minutes" are posted in a way that each
  Discord user sees them in their own time zone and language.
* **Rate limits:** Discord accepts about 30 messages per minute per channel.
  When more happens, OpenMU combines up to 10 messages into one post and waits
  for Discord, so nothing gets lost unless the queue overflows.
* **Mini games** like Blood Castle open all their levels at the same time; this
  is announced only once.
* **Mentions** are disabled, so a text of a player like `@everyone` can't ping
  anybody.
* When Discord rejects the token, the bot stops and logs an error, instead of
  trying again and again.
* Make your notices channel an *Announcement channel*: other Discord servers,
  e.g. the ones of guilds, can then *follow* it and get the messages copied into
  their own channel. `/openmu setup` does this on community servers.
* Until the bot is connected to Discord, which takes a few seconds after the
  start, notifications for the channels of the layout are posted through the
  webhook of their category, if there is one.
