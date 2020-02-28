# C1 C4 - SetFriendOnlineState (by client)

## Is sent when

A player wants to set himself on- or offline.

## Causes the following actions on the server side

Depending on the state, the player is shown as offline or online in all friend lists of his friends.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC4  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | OnlineState |