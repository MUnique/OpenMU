# C1 3D - TradeCancel (by client)

## Is sent when

The player wants to cancel the trade.

## Causes the following actions on the server side

The trade is cancelled and the previous inventory state is restored.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3D  | Packet header - packet type identifier |