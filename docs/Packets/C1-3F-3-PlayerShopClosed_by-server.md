# C1 3F 3 - PlayerShopClosed (by server)

## Is sent when

After a player in scope requested to close his shop or after all items has been sold.

## Causes the following actions on the client side

The player shop not shown as open anymore.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x3  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean | true | Success |
| 5 | 2 | ShortBigEndian |  | PlayerId |