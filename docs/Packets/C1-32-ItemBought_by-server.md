# C1 32 - ItemBought (by server)

## Is sent when

The request of buying an item from a player or npc was successful.

## Causes the following actions on the client side

The bought item is added to the inventory.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x32  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | InventorySlot |
| 4 |  | Binary |  | ItemData |