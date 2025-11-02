# C1 D2 02 - CashShopOpenState (by client)

## Is sent when

The player opens or closes the cash shop dialog.

## Causes the following actions on the server side

In case of opening, the server returns if the cash shop is available. If the player is in the safezone, it's not.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | IsClosed |