# C1 D2 14 - CashShopItemRefundResponse (by server)

## Is sent when

Response to cash shop item refund request.

## Causes the following actions on the client side

Client displays refund result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x14  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |