# C2 D2 0A - CashShopStorageListResponse (by server)

## Is sent when

Response to cash shop storage list request.

## Causes the following actions on the client side

Client displays the items in cash shop storage.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ItemCount |
| 5 | StoredItem.Length *  | Array of StoredItem |  | Items |

### StoredItem Structure

The structure for a stored item, e.g. in the inventory or vault.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | ItemSlot |
| 1 |  | Binary |  | ItemData |