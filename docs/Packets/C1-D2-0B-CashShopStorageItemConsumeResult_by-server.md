# C1 D2 0B - CashShopStorageItemConsumeResult (by server)

## Is sent when

The player requested to use an item of the cash shop storage (CashShopStorageItemConsumeRequest).

## Causes the following actions on the client side

The client shows a message with the result. If the item was used, it requests the shown storage page again.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0B  | Packet header - sub packet type identifier |
| 4 | 1 | CashShopConsumeResult |  | Result |

### CashShopConsumeResult Enum

The result of a request to use an item of the cash shop storage.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The item was used. |
| 1 | ItemNotFound | The item doesn't exist in the storage. |
| 2 | PcCafeOnly | The item can only be received in a PC cafe. |
| 3 | ColorPlanActive | A color plan is already active in the selected period. |
| 4 | PersonalFixedPlanActive | A personal fixed plan is already active in the selected period. |
| 21 | InventoryFull | The inventory doesn't have enough space. |
| 22 | CannotUse | The item can't be used. |
| 24 | ConflictingItemActive | The item can't be used together with an item which is already in use. |
| 254 | DatabaseAccessFailed | The access to the database failed. |
| 255 | Error | An error occurred. |