# C2 D2 05 - CashShopEventItemListResponse (by server)

## Is sent when

Response to event item list request.

## Causes the following actions on the client side

Client displays available event items.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ItemCount |
| 5 | CashShopProduct.Length *  | Array of CashShopProduct |  | Products |

### CashShopProduct Structure

Defines a product available in the cash shop.

Length: 16 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | ProductId |
| 4 | 4 | IntegerLittleEndian |  | Price |
| 8 | 1 | Byte |  | CoinType |
| 9 | 1 | Byte |  | CategoryIndex |
| 10 | 2 | ShortLittleEndian |  | ItemGroup |
| 12 | 2 | ShortLittleEndian |  | ItemNumber |
| 14 | 1 | Byte |  | ItemLevel |
| 15 | 1 | Byte |  | ItemDurability |