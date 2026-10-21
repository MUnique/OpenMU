# C1 D2 06 - CashShopStorageListResponse (by server)

## Is sent when

The player requested a page of the cash shop storage or gift storage (CashShopStorageListRequest).

## Causes the following actions on the client side

The client clears the storage list and expects the items of the page with the following CashShopStorageItem or CashShopGiftStorageItem messages.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | TotalItemCount |
| 6 | 2 | ShortLittleEndian |  | PageItemCount; The number of items on the requested page. The client shows up to 9 items per page. |
| 8 | 2 | ShortLittleEndian |  | PageIndex; The one-based index of the page. |
| 10 | 2 | ShortLittleEndian |  | TotalPages |