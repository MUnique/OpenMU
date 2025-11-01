# C1 C0 - FriendListRequest (by client)

## Is sent when

The client requests the current friend list.

## Causes the following actions on the server side

The server sends the friend list to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC0  | Packet header - packet type identifier |