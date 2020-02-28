# C4 F3 10 - CharacterInventory (by server)

## Is sent when

The player entered the game or finished a trade.

## Causes the following actions on the client side

The user interface of the inventory is initialized with all of its items.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC4  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x10  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | ItemCount |
| 6 | StoredItem.Length * ItemCount | Array of StoredItem |  | Items |

### StoredItem Structure

The structure for a stored item, e.g. in the inventory or vault.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | ItemSlot |
| 1 |  | Binary |  | ItemData |