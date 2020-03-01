# C1 3F 2 - PlayerShopOpenSuccessful (by server)

## Is sent when

After the player requested to open his shop and this request was successful.

## Causes the following actions on the client side

The own player shop is shown as open.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x2  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean | true | Success |