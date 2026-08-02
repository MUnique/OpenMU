# C1 B9 03 - HuntingZoneGuardInfo (by server)

## Is sent when

The server sends information about the hunting zone guard configuration.

## Causes the following actions on the client side

The client shows the hunting zone entrance configuration.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB9  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | IsEnabled |
| 5 | 4 | IntegerBigEndian |  | TaxRate |