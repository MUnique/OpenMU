# C3 36 - TradeRequest (by server)

## Is sent when

A trade was requested by another player.

## Causes the following actions on the client side

A trade request dialog is shown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x36  | Packet header - packet type identifier |
| 3 | 10 | String |  | Name |