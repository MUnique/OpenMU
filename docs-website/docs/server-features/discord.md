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
  the game servers up to date, and answers slash commands like `/who`.

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

When the bot runs, it can post the notifications as well. Configure the
identifiers of the channels instead of webhooks, e.g.
`Discord__Bot__Channels__Events=123456789012345678`. A category with a channel of
the bot is posted by the bot, even when it also has a webhook.

To copy the identifier of a channel, enable the *Developer Mode* in the advanced
settings of Discord, then right-click the channel and select *Copy Channel ID*.

## The bot

### Creating the bot

1. Open the [Discord developer portal](https://discord.com/developers/applications),
   create a *New Application* and give it the name of your server.
2. Under *Bot*, click *Reset Token* and copy the token. Treat it like a password.
   The bot doesn't need any of the *Privileged Gateway Intents*.
3. Under *OAuth2 → URL Generator*, select the scopes `bot` and
   `applications.commands`, and the permissions *View Channels*, *Send Messages*,
   *Embed Links* and *Read Message History*. Open the generated URL to invite the
   bot to your Discord server.
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

When you configure the identifier of your Discord server (`Discord__Bot__GuildId`),
the commands are available immediately. Otherwise, Discord may take up to an hour
to show them.

### Status

The presence of the bot shows how many players are online. When you configure a
status channel (`Discord__Bot__StatusChannelId`), the bot keeps a message with
the status of the game servers up to date in it. Use a channel in which only the
bot writes; it finds its message again after a restart.

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
| `Bot__GuildId` | — | The identifier of your Discord server, for the slash commands. |
| `Bot__StatusChannelId` | — | The identifier of the channel with the status message. |
| `Bot__Channels__<Category>` | — | The identifier of the channel per category, into which the bot posts the notifications. |
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
  their own channel.
