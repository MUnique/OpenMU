# C1 34 - RepairItemRequest (by client)

## Is sent when

A player wants to repair an item of his inventory.

## Causes the following actions on the server side

The item is repaired if the player has enough money in its inventory. A corresponding response is sent.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x34  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | ItemSlot; Inventory item slot of the target item. If it's 0xFF, the player wants to repair all items - this is only possible with some opened NPC dialogs. Repairing the pet item slot (8) is only possible when the pet trainer npc is opened. |
| 4 | 1 | Boolean |  | IsSelfRepair; If the player repairs it over his inventory, it's true. However, a server should never rely on this flag and do his own checks. |