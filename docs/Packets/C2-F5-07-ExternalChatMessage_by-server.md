# C2 F5 07 - ExternalChatMessage (by server)

## Is sent when

A message was written outside of the game, e.g. in a Discord channel which is bound to the chat of a guild, an alliance or the world chat. It's only sent to clients which requested the DiscordIntegrationInfo before; other clients get it as normal chat message, with a prefix in front of the sender.

## Causes the following actions on the client side

The client shows the message in the chat of its scope, marked with its source, so that it can't be mistaken for a message of a character.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 5 | 1 | ExternalChatSource |  | Source |
| 6 | 1 | ExternalChatScope |  | Scope |
| 7 | 48 | String |  | SenderName; The name of the sender, without a prefix. It's not a character name, so it can't be whispered. |
| 55 |  | String |  | Message |

### ExternalChatSource Enum

Where an external chat message was written.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Discord | The message was written in Discord. |

### ExternalChatScope Enum

The chat of the game which an external chat message belongs to.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Guild | The chat of the guild. |
| 1 | Alliance | The chat of the alliance. |
| 2 | World | The world chat, which all players of all game servers can read. |