# C1 BD 04 - CrywolfLeftTime (by server)

## Is sent when

Every 20 seconds during the battle.

## Causes the following actions on the client side

The client shows the remaining minutes and counts down the seconds by itself. It expects this packet every 20 seconds: when the minute is the same as before, it assumes that 40 or 20 seconds of the minute are left.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Hours; The remaining hours. The client doesn't show them. |
| 5 | 1 | Byte |  | Minutes; The remaining minutes. |