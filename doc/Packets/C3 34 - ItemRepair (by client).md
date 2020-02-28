# C3 34 - ItemRepair (by client)

## Is sent when

A player wants to repair an item of his inventory, either himself or with the usage of an NPC.

## Causes the following actions on the server side

If the item is damaged and repairable, the durability of the item is maximized and corresponding responses are sent back to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x34  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | InventoryItemSlot; The inventory slot of the target item. If it's 0xFF, the player requests to repair all items with the help of an NPC. If it's 8 (Pet slot), using the pet trainer NPC is mandatory, too. |