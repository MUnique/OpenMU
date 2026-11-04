# C1 D2 02 - CashShopOpenStateResponse (by server)

## Is sent when

The player requested to open the cash shop dialog (CashShopOpenState).

## Causes the following actions on the client side

If the opening is allowed, the client requests the point information and the first storage page, and shows the cash shop dialog. Otherwise, nothing happens.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | IsAllowed |