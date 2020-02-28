# C1 54 - ShowGuildMasterDialog (by server)

## Is sent when

After a player started talking to the guild master NPC and the player is allowed to create a guild.

## Causes the following actions on the client side

The client shows the guild master dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x54  | Packet header - packet type identifier |