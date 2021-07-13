# C2 45 - AddTransformedCharactersToScope075 (by server)

## Is sent when

The player wears a monster transformation ring.

## Causes the following actions on the client side

The character appears as monster, defined by the Skin property.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x45  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | CharacterCount |
| 5 | CharacterData.Length * CharacterCount | Array of CharacterData |  | Characters |

### CharacterData Structure

Contains the data of an character.

Length: 19 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | CurrentPositionX |
| 3 | 1 | Byte |  | CurrentPositionY |
| 4 | 1 | Byte |  | Skin |
| 5 << 0 | 1 bit | Boolean |  | IsPoisoned |
| 5 << 1 | 1 bit | Boolean |  | IsIced |
| 5 << 2 | 1 bit | Boolean |  | IsDamageBuffed |
| 5 << 3 | 1 bit | Boolean |  | IsDefenseBuffed |
| 6 | 10 | String |  | Name |
| 16 | 1 | Byte |  | TargetPositionX |
| 17 | 1 | Byte |  | TargetPositionY |
| 18 | 4 bit | Byte |  | Rotation |
| 18 << 0 | 4 bit | CharacterHeroState |  | HeroState |

### CharacterHeroState Enum

Defines the hero state of a character.

| Value | Name | Description |
|-------|------|-------------|
| 0 | New | The character is new and has the highest state. |
| 1 | Hero | The character is a hero. |
| 2 | LightHero | The character is a hero, but the state is almost gone. |
| 3 | Normal | The character is in a neutral state. |
| 4 | PlayerKillWarning | The character killed another character, and has a kill warning. |
| 5 | PlayerKiller1stStage | The character killed two characters, and has some restrictions. |
| 6 | PlayerKiller2ndStage | The character killed more than two characters, and has hard restrictions. |