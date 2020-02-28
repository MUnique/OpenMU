# C3 3F 03 - PlayerShopClose (by client)

## Is sent when

The player wants to close his personal item shop.

## Causes the following actions on the server side

The personal item shop is closed and the surrounding players are informed about it, including the own player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3F  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |