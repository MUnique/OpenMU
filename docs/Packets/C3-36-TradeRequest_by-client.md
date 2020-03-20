# C3 36 - TradeRequest (by client)

## Is sent when

The player requests to open a trade with another player.

## Causes the following actions on the server side

The request is forwarded to the requested player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x36  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | PlayerId |