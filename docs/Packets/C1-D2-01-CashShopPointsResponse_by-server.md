# C1 D2 01 - CashShopPointsResponse (by server)

## Is sent when

Response to cash shop points request.

## Causes the following actions on the client side

Client displays the available cash points.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | WCoinC |
| 8 | 4 | IntegerLittleEndian |  | WCoinP |
| 12 | 4 | IntegerLittleEndian |  | GoblinPoints |