# C3 3F 07 - PlayerShopCloseOther (by client)

## Is sent when

A player closes the dialog of another players shop.

## Causes the following actions on the server side

The server handles that by unsubscribing the player from changes of the shop.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 10 | String |  | PlayerName |