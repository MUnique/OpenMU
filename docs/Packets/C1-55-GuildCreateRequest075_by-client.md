# C1 55 - GuildCreateRequest075 (by client)

## Is sent when

When a player wants to create a guild.

## Causes the following actions on the server side

The guild is created and the player is set as the new guild master of the guild.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   43   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x55  | Packet header - packet type identifier |
| 3 | 8 | String |  | GuildName |
| 11 | 32 | Binary |  | GuildEmblem; The guild emblem in a custom bitmap format. It supports 16 colors (one transparent) per pixel and has a size of 8 * 8 pixel. |