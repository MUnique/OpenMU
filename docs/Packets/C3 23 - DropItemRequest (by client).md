# C3 23 - DropItemRequest (by client)

## Is sent when

A player requests to drop on item of his inventory on the ground.

## Causes the following actions on the server side

When the specified coordinates are valid, and the item is allowed to be dropped, it will be dropped on the ground and the surrounding players are notified.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x23  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | TargetX |
| 4 | 1 | Byte |  | TargetY |
| 5 | 1 | Byte |  | ItemSlot |