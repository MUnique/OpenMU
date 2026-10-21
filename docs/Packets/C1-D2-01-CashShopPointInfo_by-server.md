# C1 D2 01 - CashShopPointInfo (by server)

## Is sent when

The player requested the cash shop point information (CashShopPointInfoRequest).

## Causes the following actions on the client side

The client shows the available WCoin (C), WCoin (P) and Goblin Points in the cash shop dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   45   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ViewType; It is stored, but not used by the client. |
| 5 | 8 | Double |  | TotalCash; The sum of WCoin (C) and WCoin (P). It is stored, but not shown by the client. |
| 13 | 8 | Double |  | WCoinC; The available WCoin (C), originally named "cash credit". |
| 21 | 8 | Double |  | WCoinP; The available WCoin (P), originally named "cash prepaid". |
| 29 | 8 | Double |  | TotalPoints; It is stored, but not shown by the client. |
| 37 | 8 | Double |  | GoblinPoints; The available Goblin Points, originally named "mileage". |