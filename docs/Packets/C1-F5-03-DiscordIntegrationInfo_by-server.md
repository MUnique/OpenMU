# C1 F5 03 - DiscordIntegrationInfo (by server)

## Is sent when

After the client requested the Discord integration info, and after the player unlinked the account.

## Causes the following actions on the client side

The client configures its Discord features with it: the Rich Presence, a button which opens the invite, the account link dialog and the notice that the chat is mirrored.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   224   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | IsAccountLinked; Defines if the account is linked to a Discord user. |
| 5 | 1 | Boolean |  | IsGuildChatBridged; Defines if the chat of the guild of the character is mirrored to a Discord channel. |
| 6 | 1 | Boolean |  | IsAllianceChatBridged; Defines if the chat of the alliance of the character is mirrored to a Discord channel. |
| 7 | 1 | Boolean |  | IsWorldChatBridged; Defines if the world chat is mirrored to a Discord channel. |
| 8 | 20 | String |  | RichPresenceApplicationId; The id of the Discord application which the client uses for the Rich Presence. Empty, if the server has none. |
| 28 | 32 | String |  | RichPresenceLargeImageKey; The key of the large image of the Rich Presence, as uploaded to the Discord application. Empty for no image. |
| 60 | 32 | String |  | RichPresenceSmallImageKey; The key of the small image of the Rich Presence, as uploaded to the Discord application. Empty for no image. |
| 92 | 100 | String |  | InviteUrl; The invite link to the Discord server of the game server, e.g. 'https://discord.gg/abc123'. Empty, if the server has none. |
| 192 | 32 | String |  | LinkedUserName; The name of the Discord user which the account is linked to. Empty, if the account isn't linked. |