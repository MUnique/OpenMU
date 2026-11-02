# C1 D1 02 - KanturuRefineryState (by server)

## Is sent when

During the kanturu event.

## Causes the following actions on the client side

The client updates the state of the third kanturu stage.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | State |
| 5 | 1 | Byte |  | DetailState |