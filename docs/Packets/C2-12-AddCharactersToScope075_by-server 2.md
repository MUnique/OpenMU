# C2 12 - AddCharactersToScope075 (by server)

## Is sent when

One or more character got into the observed scope of the player.

## Causes the following actions on the client side

The client adds the character to the shown map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x12  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | CharacterCount |
| 5 | CharacterData.Length * CharacterCount | Array of CharacterData |  | Characters |

### CharacterData Structure

Contains the data of an NPC.

Length: 27 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | CurrentPositionX |
| 3 | 1 | Byte |  | CurrentPositionY |
| 4 | 9 | Binary |  | Appearance |
| 13 << 0 | 1 bit | Boolean |  | IsPoisoned |
| 13 << 1 | 1 bit | Boolean |  | IsIced |
| 13 << 2 | 1 bit | Boolean |  | IsDamageBuffed |
| 13 << 3 | 1 bit | Boolean |  | IsDefenseBuffed |
| 14 | 10 | String |  | Name |
| 24 | 1 | Byte |  | TargetPositionX |
| 25 | 1 | Byte |  | TargetPositionY |
| 26 | 4 bit | Byte |  | Rotation |
| 26 << 0 | 4 bit | CharacterHeroState |  | HeroState |

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