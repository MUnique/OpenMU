# C1 F3 22 - GuildSoccerTimeUpdate (by server)

## Is sent when

Every second during a guild soccer match.

## Causes the following actions on the client side

The time is updated on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x22  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | Seconds |