# C1 F4 02 - ServerListRequestOld (by client)

## Is sent when

This packet is sent by the client (below season 1) after it connected and received the 'Hello' message.

## Causes the following actions on the server side

The server will send a ServerListResponseOld back to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF4  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |