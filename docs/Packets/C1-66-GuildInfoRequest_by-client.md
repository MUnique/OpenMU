# C1 66 - GuildInfoRequest (by client)

## Is sent when

A player gets another player into view range which is in a guild, and the guild identifier is unknown (=not cached yet by previous requests) to him.

## Causes the following actions on the server side

The server sends a response which includes the guild name and emblem.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x66  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | GuildId |