# C1 D2 14 - CashShopItemRefundRequest (by client)

## Is sent when

The player wants to refund an item from the cash shop storage.

## Causes the following actions on the server side

The server refunds the item and returns cash points to the player's account.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x14  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ItemSlot |