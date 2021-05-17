# C2 1F - AddSummonedMonstersToScope075 (by server)

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

Length: 19 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | TypeNumber |
| 3 << 0 | 1 bit | Boolean |  | IsPoisoned |
| 3 << 1 | 1 bit | Boolean |  | IsIced |
| 3 << 2 | 1 bit | Boolean |  | IsDamageBuffed |
| 3 << 3 | 1 bit | Boolean |  | IsDefenseBuffed |
| 4 | 1 | Byte |  | CurrentPositionX |
| 5 | 1 | Byte |  | CurrentPositionY |
| 6 | 1 | Byte |  | TargetPositionX |
| 7 | 1 | Byte |  | TargetPositionY |
| 8 | 4 bit | Byte |  | Rotation |
| 9 | 10 | String |  | OwnerCharacterName |