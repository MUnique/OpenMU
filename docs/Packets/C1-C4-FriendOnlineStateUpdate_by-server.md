# C1 C4 - FriendOnlineStateUpdate (by server)

## Is sent when

After a friend has been added to the friend list.

## Causes the following actions on the client side

The friend appears in the friend list.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   14   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC4  | Packet header - packet type identifier |
| 3 | 10 | String |  | FriendName |
| 13 | 1 | Byte |  | ServerId; The server id on which the player currently is online. 0xFF means offline. |