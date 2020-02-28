# C1 CA - ChatRoomCreateRequest (by client)

## Is sent when

A player wants to open a chat with another player of his friend list.

## Causes the following actions on the server side

If both players are online, a chat room is created on the chat server. Authentication data is sent to both game clients, which will then try to connect to the chat server using this data.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xCA  | Packet header - packet type identifier |
| 3 | 10 | String |  | FriendName |