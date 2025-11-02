# C1 3B - TradeMoneyUpdate (by server)

## Is sent when

This message is sent when the trading partner put a certain amount of money (also 0) into the trade.

## Causes the following actions on the client side

It overrides all previous sent money values.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3B  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | MoneyAmount |