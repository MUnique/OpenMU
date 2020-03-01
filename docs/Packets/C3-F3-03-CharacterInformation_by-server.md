# C3-F3-03-CharacterInformation_by-server

## Is sent when

After the character was selected by the player and entered the game.

## Causes the following actions on the client side

The characters enters the game world.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   72   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | X |
| 5 | 1 | Byte |  | Y |
| 6 | 2 | ShortBigEndian |  | MapId |
| 8 | 8 | Long |  | CurrentExperience |
| 16 | 8 | Long |  | ExperienceForNextLevel |
| 24 | 2 | ShortBigEndian |  | LevelUpPoints |
| 26 | 2 | ShortBigEndian |  | Strength |
| 28 | 2 | ShortBigEndian |  | Agility |
| 30 | 2 | ShortBigEndian |  | Vitality |
| 32 | 2 | ShortBigEndian |  | Energy |
| 34 | 2 | ShortBigEndian |  | CurrentHealth |
| 36 | 2 | ShortBigEndian |  | MaximumHealth |
| 38 | 2 | ShortBigEndian |  | CurrentMana |
| 40 | 2 | ShortBigEndian |  | MaximumMana |
| 42 | 2 | ShortBigEndian |  | CurrentShield |
| 44 | 2 | ShortBigEndian |  | MaximumShield |
| 46 | 2 | ShortBigEndian |  | CurrentAbility |
| 48 | 2 | ShortBigEndian |  | MaximumAbility |
| 52 | 4 | IntegerBigEndian |  | Money |
| 56 | 1 | CharacterHeroState |  | HeroState |
| 57 | 1 | CharacterStatus |  | Status |
| 58 | 2 | ShortBigEndian |  | UsedFruitPoints |
| 60 | 2 | ShortBigEndian |  | MaxFruitPoints |
| 62 | 2 | ShortBigEndian |  | Leadership |
| 64 | 2 | ShortBigEndian |  | UsedNegativeFruitPoints |
| 66 | 2 | ShortBigEndian |  | MaxNegativeFruitPoints |
| 68 | 1 | Boolean |  | IsVaultExtended |

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

### CharacterStatus Enum

The status of a character.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | The state of the character is normal. |
| 1 | Banned | The character is banned from the game. |
| 32 | GameMaster | The character is a game master. |