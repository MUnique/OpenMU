# C2 13 D5 - AddNpcsToScopeGlobal (by server)

## Is sent when

NPCs enter scope in a global coordinate world.

## Causes the following actions on the client side

The client adds NPCs using ushort coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x13  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0xD5  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | NpcCount |
| 6 | NpcDataGlobal.Length * NpcCount | Array of NpcDataGlobal |  | NPCs |

### NpcDataGlobal Structure

NPC data with global ushort coordinates.

Length: 14 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 2 | ShortBigEndian |  | TypeNumber |
| 4 | 2 | ShortBigEndian |  | CurrentPositionX |
| 6 | 2 | ShortBigEndian |  | CurrentPositionY |
| 8 | 2 | ShortBigEndian |  | TargetPositionX |
| 10 | 2 | ShortBigEndian |  | TargetPositionY |
| 12 | 4 bit | Byte |  | Rotation |
| 13 | 1 | Byte |  | EffectCount |