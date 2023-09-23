# C1 71 - PingResponse (by client)

## Is sent when

After the server sent a ping request.

## Causes the following actions on the server side

The server knows the latency between server and client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x71  | Packet header - packet type identifier |