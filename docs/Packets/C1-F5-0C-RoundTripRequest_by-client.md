# C1 F5 0C - RoundTripRequest (by client)

## Is sent when

The client measures the time a message needs to the server and back, e.g. to show the player's ping.

## Causes the following actions on the server side

The server sends a RoundTripResponse with the same token right away.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0C  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Token; A value of the client's choice, which the server sends back unchanged, so that the client can match the response to its request. |