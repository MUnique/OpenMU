# C1 71 - PingRequest (by server)

## Is sent when

The server checks if the client is still responsive.

## Causes the following actions on the client side

The client answers immediately with C1 71.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x71  | Packet header - packet type identifier |