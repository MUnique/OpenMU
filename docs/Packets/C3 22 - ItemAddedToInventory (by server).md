# C3 22 - ItemAddedToInventory (by server)

## Is sent when

A new item was added to the inventory.

## Causes the following actions on the client side

The client adds the item to the inventory user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x22  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | InventorySlot |
| 4 |  | Binary |  | ItemData |