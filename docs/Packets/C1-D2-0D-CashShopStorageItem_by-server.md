# C1 D2 0D - CashShopStorageItem (by server)

## Is sent when

After CashShopStorageListResponse of the normal storage, for each item of the requested page.

## Causes the following actions on the client side

The client adds the item to the shown storage list. Name, quantity and period are looked up in the cash shop script by the product and price sequence numbers.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   33   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0D  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | StorageIndex; The index of the item in the storage. The client sends it back when the item should be used. |
| 8 | 4 | IntegerLittleEndian |  | ItemSequence; The client sends it back when the item should be used. |
| 12 | 4 | IntegerLittleEndian |  | StorageGroupCode; It is stored, but not used by the client. |
| 16 | 4 | IntegerLittleEndian |  | ProductSequence; The product sequence number of the cash shop script. |
| 20 | 4 | IntegerLittleEndian |  | PriceSequence; The price sequence number of the cash shop script. If it's 0, the client takes the data of the first entry of the product. |
| 24 | 8 | Double |  | CashPoints; The amount of WCoin, if the item type is Cash. |
| 32 | 1 | CashShopStorageItemType |  | ItemType |

### CashShopStorageItemType Enum

The type of an item in the cash shop storage.

| Value | Name | Description |
|-------|------|-------------|
| 67 | Cash | The item is an amount of WCoin, which is defined by the cash points field. The ASCII character 'C'. |
| 80 | Product | The item is a product of the cash shop script. The ASCII character 'P'. |