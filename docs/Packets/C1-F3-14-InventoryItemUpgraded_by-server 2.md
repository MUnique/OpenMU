# C1 F3 14 - InventoryItemUpgraded (by server)

## Is sent when

An item in the inventory got upgraded by the player, e.g. by applying a jewel.

## Causes the following actions on the client side

The item is updated on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x14  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | InventorySlot |
| 5 |  | Binary |  | ItemData |