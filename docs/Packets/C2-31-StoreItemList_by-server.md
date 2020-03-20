# C2 31 - StoreItemList (by server)

## Is sent when

The player opens a merchant npc or the vault. It's sent after the dialog was opened by another message.

## Causes the following actions on the client side

The client shows the items in the opened dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x31  | Packet header - packet type identifier |
| 4 | 2 | ShortBigEndian |  | ItemCount |
| 6 | StoredItem.Length * ItemCount | Array of StoredItem |  | Items |

### StoredItem Structure

The structure for a stored item, e.g. in the inventory or vault.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | ItemSlot |
| 1 |  | Binary |  | ItemData |