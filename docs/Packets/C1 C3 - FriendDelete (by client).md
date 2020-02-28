# C1 C3 - FriendDelete (by client)

## Is sent when

A player wants to delete another players character from his friend list of the messenger.

## Causes the following actions on the server side

The entry in the friend list is removed. The player is shown as offline in the other players friends list.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC3  | Packet header - packet type identifier |
| 3 | 10 | String |  | FriendName |