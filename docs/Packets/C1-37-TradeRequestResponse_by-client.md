# C1 37 - TradeRequestResponse (by client)

## Is sent when

A requested player responded to a trade request of another player.

## Causes the following actions on the server side

When the trade request was accepted, the server tries to open a new trade and sends corresponding responses to both players. 

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x37  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | TradeAccepted |