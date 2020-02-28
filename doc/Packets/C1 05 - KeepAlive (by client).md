# C1 05 - KeepAlive (by client)

## Is sent when

This packet is sent by the client in a fixed interval.

## Causes the following actions on the server side

The server will keep the connection and chat room intact as long as the clients sends a message in a certain period of time.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x05  | Packet header - packet type identifier |