# C1 C1 - FriendAddRequest (by client)

## Is sent when

A player wants to add another players character into his friend list of the messenger.

## Causes the following actions on the server side

A request is sent to the other player. If the player is currently offline, the request will be sent as soon as he is online again.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC1  | Packet header - packet type identifier |
| 3 | 10 | String |  | FriendName |