# C1 3A - SetTradeMoney (by client)

## Is sent when

A player requests to set an amount of money in the trade.

## Causes the following actions on the server side

It's taken from the available money of the inventory. If the new money amount is lower than the amount which was set before, it's added back to the inventory. The trade partner is informed about any change.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3A  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Amount |