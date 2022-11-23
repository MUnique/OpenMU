# C2 3F 00 - PlayerShops (by server)

## Is sent when

After the player gets into scope of a player with an opened shop.

## Causes the following actions on the client side

The player shop title is shown at the specified players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | ShopCount |
| 6 | PlayerShop.Length * ShopCount | Array of PlayerShop |  | Shops |

### PlayerShop Structure

Data of the shop of a player.

Length: 38 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | PlayerId |
| 2 | 36 | String |  | StoreName |