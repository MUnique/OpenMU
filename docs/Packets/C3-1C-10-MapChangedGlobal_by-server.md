# C3 1C 10 - MapChangedGlobal (by server)

## Is sent when

The map or global position changed.

## Causes the following actions on the client side

The client changes map and position using ushort coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1C  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x10  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean | true | IsMapChange |
| 5 | 2 | ShortBigEndian |  | MapNumber |
| 7 | 2 | ShortBigEndian |  | PositionX |
| 9 | 2 | ShortBigEndian |  | PositionY |
| 11 | 1 | Byte |  | Rotation |