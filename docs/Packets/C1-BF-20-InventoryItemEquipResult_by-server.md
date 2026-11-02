# C1 BF 20 - InventoryItemEquipResult (by server)

## Is sent when

After the player requested to (un)equip an item which is used inside the inventory (BF 20).

## Causes the following actions on the client side

The client updates the state of the item.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x20  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | InventorySlot |
| 5 | 1 | Byte |  | Result |