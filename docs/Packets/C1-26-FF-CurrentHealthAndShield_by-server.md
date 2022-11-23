# C1 26 FF - CurrentHealthAndShield (by server)

## Is sent when

Periodically, or if the current health or shield changed on the server side, e.g. by hits.

## Causes the following actions on the client side

The health and shield bar is updated on the game client user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x26  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0xFF  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | Health |
| 7 | 2 | ShortBigEndian |  | Shield |