# C3 3F 06 - PlayerShopItemBuyRequest (by client)

## Is sent when

A player wants to buy the item of another players shop.

## Causes the following actions on the server side

If the buyer has enough money, the item is sold to the player. Both players will get notifications about that.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 10 | String |  | PlayerName |
| 16 | 1 | Byte |  | ItemSlot |