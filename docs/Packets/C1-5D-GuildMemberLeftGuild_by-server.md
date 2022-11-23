# C1 5D - GuildMemberLeftGuild (by server)

## Is sent when

A player left a guild. This message is sent to the player and all surrounding players.

## Causes the following actions on the client side

The player is not longer shown as a guild member.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x5D  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | PlayerId |
| 3 << 7 | 1 bit | Boolean |  | IsGuildMaster |