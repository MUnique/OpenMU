# C1 CB - ChatRoomInvitationRequest (by client)

## Is sent when

A player wants to invite additional players from his friend list to an existing chat room.

## Causes the following actions on the server side

The player additional gets authentication data sent to his game client. It then connects to the chat server and joins the chat room.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xCB  | Packet header - packet type identifier |
| 3 | 10 | String |  | FriendName |
| 13 | 2 | ShortBigEndian |  | RoomId |
| 15 | 4 | IntegerBigEndian |  | RequestId |