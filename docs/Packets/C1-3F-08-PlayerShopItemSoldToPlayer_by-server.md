# C1 3F 08 - PlayerShopItemSoldToPlayer (by server)

## Is sent when

An item of the players shop was sold to another player.

## Causes the following actions on the client side

The item is removed from the players inventory and a blue system message appears.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   15   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x08  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | InventorySlot |
| 5 | 10 | String |  | BuyerName |