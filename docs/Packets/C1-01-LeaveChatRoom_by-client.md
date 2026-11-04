# C1 01 - LeaveChatRoom (by client)

## Is sent when

This packet is sent by the client when it leaves the chat room, before the connection closes.

## Causes the following actions on the server side

The server will remove the client from the chat room, notifying the remaining clients.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x01  | Packet header - packet type identifier |