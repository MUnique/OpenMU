# C1 F5 0D - RoundTripResponse (by server)

## Is sent when

Right after the client sent a RoundTripRequest.

## Causes the following actions on the client side

The client takes the time since it sent the request with this token as the round trip time, e.g. to show the player's ping.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0D  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Token; The token of the request. |