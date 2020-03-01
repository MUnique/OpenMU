# C3 24 FF - ItemMoveRequestFailed (by server)

## Is sent when

An item in the inventory or vault of the player could not be moved as requested by the player.

## Causes the following actions on the client side

The client restores the position of item in the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x24  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0xFF  | Packet header - sub packet type identifier |
| 5 |  | Binary |  | ItemData |