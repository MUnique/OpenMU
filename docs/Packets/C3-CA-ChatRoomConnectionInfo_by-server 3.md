# C3 CA - ChatRoomConnectionInfo (by server)

## Is sent when

The player is invited to join a chat room on the chat server.

## Causes the following actions on the client side

The game client connects to the chat server and joins the chat room with the specified authentication data.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   36   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xCA  | Packet header - packet type identifier |
| 3 | 15 | String |  | ChatServerIp |
| 18 | 2 | ShortLittleEndian |  | ChatRoomId |
| 20 | 4 | IntegerLittleEndian |  | AuthenticationToken |
| 24 | 1 | Byte | 1 | Type |
| 25 | 10 | String |  | FriendName |
| 35 | 1 | Boolean |  | Success |