# C3 32 - BuyItemFromNpcRequest (by client)

## Is sent when

A player wants to buy an item from an opened NPC merchant.

## Causes the following actions on the server side

If the player has enough money, the item is added to the inventory and money is removed. Corresponding messages are sent back to the game client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x32  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | ItemSlot; Item Slot (NPC Store) |