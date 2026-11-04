# C2 1F - AddSummonedMonstersToScope (by server)

## Is sent when

One or more summoned monsters got into the observed scope of the player.

## Causes the following actions on the client side

The client adds the monsters to the shown map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x1F  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | MonsterCount |
| 5 | SummonedMonsterData.Length * MonsterCount | Array of SummonedMonsterData |  | SummonedMonsters |

### SummonedMonsterData Structure

Contains the data of an NPC.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 2 | ShortBigEndian |  | TypeNumber |
| 4 | 1 | Byte |  | CurrentPositionX |
| 5 | 1 | Byte |  | CurrentPositionY |
| 6 | 1 | Byte |  | TargetPositionX |
| 7 | 1 | Byte |  | TargetPositionY |
| 8 | 4 bit | Byte |  | Rotation |
| 9 | 10 | String |  | OwnerCharacterName |
| 19 | 1 | Byte |  | EffectCount; Defines the number of effects which would be sent after this field. This is currently not supported. |
| 20 | EffectId.Length * EffectCount | Array of EffectId |  | Effects |

### EffectId Structure

Contains the id of a magic effect.

Length: 1 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Id |