# C3 1C 0F - MapChangedExtended (by server)

## Is sent when

The map was changed on the server side.

## Causes the following actions on the client side

The game client changes to the specified map and coordinates, and to the variant of the map with the specified discriminator.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1C  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0F  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean | true | IsMapChange; If false, it shows the teleport animation (white bubbles), and the client doesn't remove all of the objects in its scope. |
| 5 | 2 | ShortBigEndian |  | MapNumber |
| 7 | 1 | Byte |  | PositionX |
| 8 | 1 | Byte |  | PositionY |
| 9 | 1 | Byte |  | Rotation |
| 15 | 1 | Byte |  | MapDiscriminator; The discriminator of the map, which tells maps with the same number apart, e.g. the stage of Devil Square (1 to 7). It's appended after the 15 bytes of MapChanged, so a client can tell by the packet length whether it was sent. |