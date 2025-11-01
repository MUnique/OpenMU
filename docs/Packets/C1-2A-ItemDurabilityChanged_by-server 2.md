# C1 2A - ItemDurabilityChanged (by server)

## Is sent when

The durability of an item in the inventory of the player has been changed.

## Causes the following actions on the client side

The client updates the item in the inventory user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x2A  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | InventorySlot |
| 4 | 1 | Byte |  | Durability |
| 5 | 1 | Boolean |  | ByConsumption; true, if the change resulted from an item consumption; otherwise, false |