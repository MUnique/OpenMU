# C1 53 - GuildKickPlayerRequest (by client)

## Is sent when

A guild member wants to kick himself or a guild master wants to kick another player from its guild.

## Causes the following actions on the server side

If the player is allowed to kick the player, it's removed from the guild. If the guild master kicks himself, the guild is disbanded. Corresponding responses are sent to all involved players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x53  | Packet header - packet type identifier |
| 3 | 10 | String |  | PlayerName |
| 13 |  | String |  | SecurityCode |