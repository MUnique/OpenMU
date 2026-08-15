# C1 B2 1B - CastleSiegeMiniMapResponse (by server)

## Is sent when

The server responds to a Castle Siege mini-map request.

## Causes the following actions on the client side

The client opens the mini map when the request was accepted.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x1B  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result; The result code. The client currently ignores it; historically documented values are not yet verified. |