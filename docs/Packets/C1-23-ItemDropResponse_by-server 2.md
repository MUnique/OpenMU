# C1 23 - ItemDropResponse (by server)

## Is sent when

The player requested to drop an item of his inventory. This message is the response about the success of the request.

## Causes the following actions on the client side

If successful, the client removes the item from the inventory user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x23  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success |
| 4 | 1 | Byte |  | InventorySlot |