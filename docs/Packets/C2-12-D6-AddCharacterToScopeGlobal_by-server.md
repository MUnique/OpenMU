# C2 12 D6 - AddCharacterToScopeGlobal (by server)

## Is sent when

A character entered the observed scope in the global coordinate world.

## Causes the following actions on the client side

The client adds the character using absolute ushort coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x12  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0xD6  | Packet header - sub packet type identifier |
| 5 | 2 | ShortLittleEndian |  | Id |
| 7 | 2 | ShortLittleEndian |  | CurrentPositionX |
| 9 | 2 | ShortLittleEndian |  | CurrentPositionY |
| 11 | 2 | ShortLittleEndian |  | TargetPositionX |
| 13 | 2 | ShortLittleEndian |  | TargetPositionY |
| 15 | 4 bit | Byte |  | Rotation |
| 15 << 0 | 4 bit | CharacterHeroState |  | HeroState |
| 17 | 2 | ShortLittleEndian |  | AttackSpeed |
| 19 | 2 | ShortLittleEndian |  | MagicSpeed |
| 21 | 10 | String |  | Name |
| 31 |  | Binary |  | AppearanceAndEffects |

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