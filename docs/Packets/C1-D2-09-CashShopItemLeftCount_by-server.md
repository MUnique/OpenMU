# C1 D2 09 - CashShopItemLeftCount (by server)

## Is sent when

After a request of the remaining count of a limited item.

## Causes the following actions on the client side

The 1.04d client does not read any payload of this packet.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |