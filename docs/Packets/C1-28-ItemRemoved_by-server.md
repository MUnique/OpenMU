# C1 28 - ItemRemoved (by server)

## Is sent when

The item has been removed from the inventory of the player.

## Causes the following actions on the client side

The client removes the item in the inventory user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x28  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | InventorySlot; The affected slot of the item in the inventory. |
| 4 | 1 | Byte | 1 | TrueFlag |