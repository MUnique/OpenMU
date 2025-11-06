# C1 D2 03 - CashShopItemBuyResponse (by server)

## Is sent when

Response to cash shop item purchase request.

## Causes the following actions on the client side

Client displays purchase result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 4 | IntegerLittleEndian |  | ProductId |