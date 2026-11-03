# C1 3F 10 - PlayerShopTitleChanged (by server)

## Is sent when

After a player changed the title of his personal store.

## Causes the following actions on the client side

The client updates the shop title above the player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   52   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x10  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 36 | String |  | ShopTitle |
| 42 | 10 | String |  | PlayerName |