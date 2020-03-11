# C3 3F 05 - PlayerShopItemListRequest (by client)

## Is sent when

A player opens a shop of another player.

## Causes the following actions on the server side

The list of items is sent back, if the shop of the player is currently open.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 10 | String |  | PlayerName |