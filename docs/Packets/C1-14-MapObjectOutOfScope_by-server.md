# C1 14 - MapObjectOutOfScope (by server)

## Is sent when

One or more objects (player, npc, etc.) on the map got out of scope, e.g. when the own player moved away from it/them or the object itself moved.

## Causes the following actions on the client side

The game client removes the objects from the game map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x14  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | ObjectCount |
| 4 | ObjectId.Length * ObjectCount | Array of ObjectId |  | Objects |

### ObjectId Structure

Contains the id of a object.

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |