# C1 BF 08 - IllusionTempleHolyItemRelics (by server)

## Is sent when

?

## Causes the following actions on the client side

?.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x08  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | UserIndex |
| 6 |  | String |  | Name |