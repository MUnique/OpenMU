# C1 51 - GuildJoinResponse (by client)

## Is sent when

A guild master responded to a previously sent request.

## Causes the following actions on the server side

If the request was accepted by the guild master, the previously requesting player is added to the guild.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x51  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Accepted |
| 4 | 2 | ShortBigEndian |  | RequesterId |