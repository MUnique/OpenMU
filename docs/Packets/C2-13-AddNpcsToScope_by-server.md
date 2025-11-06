# C2 13 - AddNpcsToScope (by server)

## Is sent when

One or more NPCs got into the observed scope of the player.

## Causes the following actions on the client side

The client adds the NPCs to the shown map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x13  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | NpcCount |
| 5 | NpcData.Length * NpcCount | Array of NpcData |  | NPCs |

### NpcData Structure

Contains the data of an NPC.

Length: 10 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 2 | ShortBigEndian |  | TypeNumber |
| 4 | 1 | Byte |  | CurrentPositionX |
| 5 | 1 | Byte |  | CurrentPositionY |
| 6 | 1 | Byte |  | TargetPositionX |
| 7 | 1 | Byte |  | TargetPositionY |
| 8 | 4 bit | Byte |  | Rotation |
| 9 | 1 | Byte |  | EffectCount; Defines the number of effects which would be sent after this field. This is currently not supported. |