# C1 39 - TradeItemAdded (by server)

## Is sent when

The trading partner added an item to the trade.

## Causes the following actions on the client side

The item is added in the trade dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x39  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | ToSlot |
| 4 |  | Binary |  | ItemData |