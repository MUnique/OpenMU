# C1 B7 02 - CastleSiegeMachineRegionNotify (by server)

## Is sent when

The server notifies the player of the impact region of a siege machine.

## Causes the following actions on the client side

The client shows the impact area effect.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | TargetX |
| 5 | 1 | Byte |  | TargetY |