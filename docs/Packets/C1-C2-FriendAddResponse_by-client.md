# C1 C2 - FriendAddResponse (by client)

## Is sent when

A player received a friend request from another player and responded to it.

## Causes the following actions on the server side

If the player accepted, the friend is added to the players friend list and both players get subscribed about each others online status.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC2  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Accepted |
| 4 | 10 | String |  | FriendRequesterName |