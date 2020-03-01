# C1 38 - TradeItemRemoved (by server)

## Is sent when

The trading partner removed an item from the trade.

## Causes the following actions on the client side

The item is removed from the trade dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x38  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Slot |