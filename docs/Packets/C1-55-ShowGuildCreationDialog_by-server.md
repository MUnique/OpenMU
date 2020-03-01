# C1 55 - ShowGuildCreationDialog (by server)

## Is sent when

After a player started talking to the guild master NPC and the player proceeds to create a guild.

## Causes the following actions on the client side

The client shows the guild creation dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x55  | Packet header - packet type identifier |