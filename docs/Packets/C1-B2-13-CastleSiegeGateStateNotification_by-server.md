# C1 B2 13 - CastleSiegeGateStateNotification (by server)

## Is sent when

A castle gate is opened or closed.

## Causes the following actions on the client side

The client updates the castle gate animation and collision state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x13  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | IsOpen |
| 5 | 2 | ShortBigEndian |  | GateIndex |