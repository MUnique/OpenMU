# C2 12 - AddCharactersToScope (by server)

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

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | CurrentPositionX |
| 3 | 1 | Byte |  | CurrentPositionY |
| 4 | 18 | Binary |  | Appearance |
| 22 | 10 | String |  | Name |
| 32 | 1 | Byte |  | TargetPositionX |
| 33 | 1 | Byte |  | TargetPositionY |
| 34 | 4 bit | Byte |  | Rotation |
| 34 << 0 | 4 bit | CharacterHeroState |  | HeroState |
| 35 | 1 | Byte |  | EffectCount; Defines the number of effects which would be sent after this field. This is currently not supported. |
| 36 | EffectId.Length * EffectCount | Array of EffectId |  | Effects |

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

### EffectId Structure

Contains the id of a magic effect.

Length: 1 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Id |