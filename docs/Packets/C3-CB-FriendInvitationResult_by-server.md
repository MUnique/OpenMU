# C3 CB - FriendInvitationResult (by server)

## Is sent when

The player requested to add another player to his friend list and the server processed this request.

## Causes the following actions on the client side

The game client knows if the invitation could be sent to the other player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xCB  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success |
| 4 | 4 | IntegerBigEndian |  | RequestId |