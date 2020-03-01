# C3 33 - SellItemToNpcRequest (by client)

## Is sent when

A player wants to sell an item of his inventory to the opened NPC merchant.

## Causes the following actions on the server side

The item is sold for money to the NPC. The item is removed from the inventory and money is added. Corresponding messages are sent back to the game client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x33  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | ItemSlot; Item Slot (Inventory) |