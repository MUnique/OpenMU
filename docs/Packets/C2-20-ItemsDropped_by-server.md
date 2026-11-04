# C2 20 - ItemsDropped (by server)

## Is sent when

The items dropped on the ground.

## Causes the following actions on the client side

The client adds the items to the ground.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x20  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | ItemCount |
| 5 | DroppedItem.Length * ItemCount | Array of DroppedItem |  | Items |

### DroppedItem Structure

Contains the data about a dropped item.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 0 << 7 | 1 bit | Boolean |  | IsFreshDrop; If this flag is set, the item is added to the map with an animation and sound. Otherwise it's just added like it was already on the ground before. |
| 2 | 1 | Byte |  | PositionX |
| 3 | 1 | Byte |  | PositionY |
| 4 |  | Binary |  | ItemData |