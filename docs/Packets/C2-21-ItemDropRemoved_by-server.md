# C2 21 - ItemDropRemoved (by server)

## Is sent when

A dropped item was removed from the ground of the map, e.g. when it timed out or was picked up.

## Causes the following actions on the client side

The client removes the item from the ground of the map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x21  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | ItemCount |
| 5 | DroppedItemId.Length * ItemCount | Array of DroppedItemId |  | ItemData |

### DroppedItemId Structure

Contains the id of a dropped item.

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |