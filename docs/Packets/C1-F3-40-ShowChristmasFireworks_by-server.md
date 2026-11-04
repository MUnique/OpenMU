# C1 F3 40 - ShowChristmasFireworks (by server)

## Is sent when

E.g. when event items are dropped to the floor.

## Causes the following actions on the client side

The client shows an christmas fireworks effect at the specified coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x40  | Packet header - sub packet type identifier |
| 4 | 1 | Byte | 59 | EffectType |
| 5 | 1 | Byte |  | X |
| 6 | 1 | Byte |  | Y |