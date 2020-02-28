# C1 C3 01 - FriendDeleted (by server)

## Is sent when

After a friend has been removed from the friend list.

## Causes the following actions on the client side

The friend is removed from the friend list.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   14   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 10 | String |  | FriendName |