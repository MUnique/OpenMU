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
| `/link <code>` | Links the Discord user to the game account, see [Linking accounts](#linking-accounts) |
| `/unlink` | Removes the link to the game account |
| `/character <name>` | Selects the character of the account, as which the player appears |
| `/say <message>` | Sends a message to the chat of the game which is bound to the channel, see [Chat bridge](#chat-bridge) |
| `/guildchat bind`, `create`, `unbind` | Binds the chat of a guild or alliance to a channel, see [Chat bridge](#chat-bridge) |

The commands only show information; they don't change anything in the game.
Additionally, players can [link their account](#linking-accounts) with
`/link`, `/unlink` and `/character`, and administrators of the Discord server
can use [`/openmu setup`](#setting-up-your-discord-server).

### Linking accounts

Players can link their Discord user to their game account:

1. In the game, the player enters `/discord link`. The game shows a one-time
   code like `ABCD-EFGH`, which is valid for 10 minutes.
2. In Discord, the player enters `/link ABCD-EFGH`.

The link is to the account, not to a character. The player appears as the
character which requested the code, and chooses another character of the account
with `/character <name>` in Discord. A Discord user can be linked to one account,
and an account to one Discord user; linking again replaces the previous link.
Because Discord users are the same on every Discord server, the link counts on
every Discord server the bot is in.

`/discord` in the game shows the link, and `/discord unlink` in the game or
`/unlink` in Discord removes it. In the admin panel, the *Links* button in the
account list shows the links of an account and can remove them.

Linked users get the role *Linked Player* of the
[layout](#setting-up-your-discord-server) on your Discord server, so you can,
for example, open channels only for players. The bot needs the *Manage Roles*
permission for it, and its own role has to be above *Linked Player* in the role
list. When an administrator removes a link in the admin panel, the role isn't
removed automatically.

The answers to these commands are only visible to the player who used them.
The codes are stored as hash in the database, and every code can only be used
once.

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

## Chat bridge

The bot can mirror the chats of the game to Discord channels, in both directions:

* the **world chat**: players write into it in the game with `/world <message>`,
  and all players of all game servers read it. It's mirrored to `#world-chat` of
  the [layout](#setting-up-your-discord-server).
* the chat of a **guild** (`@` in the game) or an **alliance** (`@@`), when its
  guild master binds it to a channel.

The chat of the players is private, so nothing is mirrored by default: enable
*Publish guild, alliance and world chat* in the configuration of the *Game Event
Publisher* plugin. Players who enter the game are told when the chat of their
guild or alliance is mirrored.

### Binding the chat of a guild

The guild master links the Discord user to the account (see
[Linking accounts](#linking-accounts)) and selects the guild master character
with `/character`. Then there are two ways, which you can restrict with
`Bot__ChatBridge__BindingMode`:

* **On your Discord server:** `/guildchat create` creates a channel like
  `#guild-legends` in the category *Guilds* of the layout. Only the bot and the
  members of the guild who linked their Discord user can see it. The bot keeps
  this up to date when users link themselves and every 10 status updates. It
  needs the *Manage Channels* permission for it.
* **On the Discord server of the guild:** the guild master invites the bot to
  the Discord server of the guild, and uses `/guildchat bind` in the channel
  which should be bound. This requires the permission to manage the channel. With
  `Bot__ChatBridge__AllowedDiscordServerIds`, you can allow only some Discord
  servers.

The option `scope` of these commands chooses the chat of the alliance instead,
which only the master of an alliance can bind. `/guildchat unbind` in the channel
removes the binding; the guild master and users who manage the channel can use it.
The binding is also removed when the guild is disbanded, the channel is deleted
or the bot is removed from the Discord server.

### Writing from Discord

Only Discord users who are linked to an account can write into the game, as
their selected character, which has to be a member of the guild or alliance. The
chat ban of the account applies, and a user can send 10 messages per minute
(`Bot__ChatBridge__MaximumMessagesPerMinute`).

In the game, the messages appear with the character name and the prefix `@`,
e.g. `@Hero`, so a Discord user can't pretend to be a character in the game.
Mentions, markdown and line breaks are removed, custom emojis become `:name:`,
and long messages are cut (`Bot__ChatBridge__MaximumMessageLength`).

To read the normal messages in the channels, the bot needs the privileged
*Message Content* intent: enable it for the bot in the Discord developer portal
under *Bot → Privileged Gateway Intents*, and set
`Bot__ChatBridge__ReadMessages` to `true`. When a message can't be sent to the
game, the bot reacts with ❌ (permission *Add Reactions*), and `/say` tells why.
Without the intent, players use `/say <message>`. Discord only approves the
intent for verified bots, which are in more than 100 Discord servers, with a
reason.

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
| `Bot__ChatBridge__BindingMode` | `Both` | Where the chats of guilds can be bound: `Both`, `HostedOnly` (only channels on your Discord server) or `GuildOwnedOnly` (only on Discord servers of guilds). |
| `Bot__ChatBridge__AllowedDiscordServerIds__0`, `__1`, … | all | The Discord servers of guilds, on which chats can be bound. |
| `Bot__ChatBridge__ReadMessages` | `false` | Reads the normal messages in the bound channels. Requires the *Message Content* intent. |
| `Bot__ChatBridge__MaximumMessageLength` | `100` | The maximum length of a message from Discord to the game. |
| `Bot__ChatBridge__MaximumMessagesPerMinute` | `10` | The maximum number of messages per minute of a Discord user to the game. |

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
