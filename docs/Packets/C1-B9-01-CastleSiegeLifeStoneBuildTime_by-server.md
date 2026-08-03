# C1 B9 01 - CastleSiegeLifeStoneBuildTime (by server)

## Is sent when

The creation stage of a Castle Siege Life Stone advances.

## Causes the following actions on the client side

The client updates the Life Stone construction animation.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB9  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | NpcIndex |
| 6 | 1 | Byte |  | BuildTime; The creation stage of the Life Stone. |