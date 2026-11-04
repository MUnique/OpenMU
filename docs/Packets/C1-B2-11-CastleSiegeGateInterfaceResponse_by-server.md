# C1 B2 11 - CastleSiegeGateInterfaceResponse (by server)

## Is sent when

After a player talks to the lever of a castle gate.

## Causes the following actions on the client side

The client opens the gate operation interface when access is allowed.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x11  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 2 | ShortBigEndian |  | GateIndex |