# C1 BF 03 - IllusionTempleUserCount (by server)

## Is sent when

?

## Causes the following actions on the client side

The client shows the counts.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | UserCount1 |
| 5 | 1 | Byte |  | UserCount2 |
| 6 | 1 | Byte |  | UserCount3 |
| 7 | 1 | Byte |  | UserCount4 |
| 8 | 1 | Byte |  | UserCount5 |
| 9 | 1 | Byte |  | UserCount6 |