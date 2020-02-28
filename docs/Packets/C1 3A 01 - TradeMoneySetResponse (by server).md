# C1 3A 01 - TradeMoneySetResponse (by server)

## Is sent when

The trade money has been set by a previous request of the player.

## Causes the following actions on the client side

The money which was set into the trade by the player is updated on the UI.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3A  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |