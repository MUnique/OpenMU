# C3 1C - MapChanged075 (by server)

## Is sent when

The map was changed on the server side.

## Causes the following actions on the client side

The game client changes to the specified map and coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1C  | Packet header - packet type identifier |
| 3 | 1 | Boolean | true | IsMapChange; If false, it shows the teleport animation (white bubbles), and the client doesn't remove all of the objects in its scope. |
| 4 | 1 | Byte |  | MapNumber |
| 5 | 1 | Byte |  | PositionX |
| 6 | 1 | Byte |  | PositionY |
| 7 | 1 | Byte |  | Rotation |