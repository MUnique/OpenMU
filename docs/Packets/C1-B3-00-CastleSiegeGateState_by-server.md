# C1 B3 00 - CastleSiegeGateState (by server)

## Is sent when

The server sends the current state of a castle gate.

## Causes the following actions on the client side

The client updates the gate state display.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | GateIndex |
| 6 | 1 | Byte |  | State |