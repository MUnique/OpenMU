# C1 C2 - FriendRequest (by server)

## Is sent when

After a player has requested to add another player as friend. This other player gets this message.

## Causes the following actions on the client side

The friend request appears on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC2  | Packet header - packet type identifier |
| 3 | 10 | String |  | Requester |