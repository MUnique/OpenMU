# C3 1C 0F - MapChanged (by server)

## Is sent when

The map was changed on the server side.

## Causes the following actions on the client side

The game client changes to the specified map and coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   15   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1C  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0F  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean | true | Success |
| 5 | 2 | ShortBigEndian |  | MapNumber |
| 7 | 1 | Byte |  | PositionX |
| 8 | 1 | Byte |  | PositionY |
| 9 | 1 | Byte |  | Rotation |