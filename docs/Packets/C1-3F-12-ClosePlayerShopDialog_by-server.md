# C1 3F 12 - ClosePlayerShopDialog (by server)

## Is sent when

After the player requested to close his shop or after all items has been sold.

## Causes the following actions on the client side

The player shop dialog is closed for the shop of the specified player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x12  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | PlayerId |