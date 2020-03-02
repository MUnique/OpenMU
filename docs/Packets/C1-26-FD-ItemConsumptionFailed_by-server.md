# C1 26 FD - ItemConsumptionFailed (by server)

## Is sent when

When the consumption of an item failed.

## Causes the following actions on the client side

The game client gets a feedback about a failed consumption, and allows for do further consumption requests.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x26  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0xFD  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | Health |
| 7 | 2 | ShortBigEndian |  | Shield |