# C1 B9 03 - CastleSiegeHuntingZoneGuardInfo (by server)

## Is sent when

The server sends information about the hunting zone guard configuration.

## Causes the following actions on the client side

The client shows the hunting zone entrance configuration.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   18   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB9  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 1 | Boolean |  | IsEnabled |
| 6 | 4 | IntegerLittleEndian |  | CurrentPrice |
| 10 | 4 | IntegerLittleEndian |  | MaxPrice |
| 14 | 4 | IntegerLittleEndian |  | UnitPrice |