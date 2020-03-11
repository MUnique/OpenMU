# C1 50 - GuildJoinRequest (by client)

## Is sent when

A player (non-guild member) requests to join a guild.

## Causes the following actions on the server side

The request is forwarded to the guild master. There can only be one request at a time. If the guild master already has an open request, a corresponding response is directly sent back to the requesting player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x50  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | GuildMasterPlayerId |