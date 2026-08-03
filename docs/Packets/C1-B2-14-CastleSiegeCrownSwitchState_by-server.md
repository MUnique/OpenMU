# C1 B2 14 - CastleSiegeCrownSwitchState (by server)

## Is sent when

A player starts or stops occupying a castle crown switch.

## Causes the following actions on the client side

The client updates the crown switch interaction state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x14  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | SwitchIndex |
| 6 | 2 | ShortBigEndian |  | PlayerIndex |
| 8 | 1 | Byte |  | State |