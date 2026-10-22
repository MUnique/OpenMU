---
title: Discord notifications
sidebar_label: Discord
sidebar_position: 5
description: Post events of the game, like mini games, castle siege and rare drops, to Discord channels.
---

# Discord notifications

OpenMU can post what happens in the game to the channels of your Discord server:

| Category | What is posted |
|---|---|
| `Events` | Mini games which open (e.g. Blood Castle), winners of mini games, invasions which start and end |
| `CastleSiege` | Guild registration, the start and the end of the battle, and the new owner of the castle |
| `WorldNews` | Boss kills, excellent and ancient items which monsters dropped, characters which reach a level milestone |
| `Notices` | Global notices which game masters send with the `!` chat prefix |

Each category is posted to its own channel through a Discord *webhook*. Categories
without webhook aren't posted.

:::note
This works in the all-in-one deployment. In the distributed deployment, the
events are already published, but there is no service yet which posts them to
Discord.
:::

## Setting it up

1. **Create the webhooks.** In Discord, open the settings of a channel, go to
   *Integrations → Webhooks*, create a webhook and copy its URL. Do this for
   each channel which should receive messages.
2. **Configure the webhooks** for the server, through environment variables,
   e.g. in your `docker-compose.override.yml`:

   ```yaml
   environment:
     - Discord__Webhooks__Events=https://discord.com/api/webhooks/...
     - Discord__Webhooks__CastleSiege=https://discord.com/api/webhooks/...
     - Discord__Webhooks__WorldNews=https://discord.com/api/webhooks/...
     - Discord__Webhooks__Notices=https://discord.com/api/webhooks/...
   ```

   Treat the URLs like passwords: everybody who knows one can post into the
   channel. That's why they are not part of the game configuration in the
   database, but of the configuration of the server process.
3. **Activate the *Game Event Publisher* plugin** in the admin panel
   ([Plugins](../admin-panel/plugins.md#game-event-publisher)). It's disabled by
   default. In its configuration, you choose which events are published, which
   monsters are bosses and which levels are milestones.
4. Restart the server.

## Settings

All settings are part of the `Discord` section of the configuration, so they can
also be set in the `appsettings.json` of the server.

| Setting | Default | Description |
|---|---|---|
| `Webhooks__<Category>` | — | The webhook URL per category, see the table above. |
| `Language` | `en` | The language of the messages. English (`en`) and German (`de`) are available. Names of maps, monsters and items are translated, too. |
| `EventAnnouncingServerIds__0`, `__1`, … | all | The identifiers of the game servers which announce mini games and invasions. When several game servers run the same events, set it to one of them to avoid duplicate messages. |
| `MaximumQueuedMessages` | `100` | The number of messages per webhook which can wait to be sent. When there are more, the oldest ones are dropped. |

## Good to know

* **Times** like "the entrance closes in 5 minutes" are posted in a way that each
  Discord user sees them in their own time zone and language.
* **Rate limits:** Discord accepts about 30 messages per minute per webhook.
  When more happens, OpenMU combines up to 10 messages into one post and waits
  for Discord, so nothing gets lost unless the queue overflows.
* **Mini games** like Blood Castle open all their levels at the same time; this
  is announced only once.
* **Mentions** are disabled, so a text of a player like `@everyone` can't ping
  anybody.
* Make your notices channel an *Announcement channel*: other Discord servers,
  e.g. the ones of guilds, can then *follow* it and get the messages copied into
  their own channel.
