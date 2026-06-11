# C1 B7 03 - CastleSiegeMachineInterface (by server)

## Is sent when

The server sends the siege machine interface to a player who is operating the machine.

## Causes the following actions on the client side

The client shows the siege machine operation interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | MachineType |