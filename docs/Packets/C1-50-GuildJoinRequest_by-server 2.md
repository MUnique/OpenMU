# C1 50 - GuildJoinRequest (by server)

## Is sent when

A player requested to join a guild. This message is sent then to the guild master.

## Causes the following actions on the client side

The guild master gets a message box with the request popping up.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x50  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | RequesterId |