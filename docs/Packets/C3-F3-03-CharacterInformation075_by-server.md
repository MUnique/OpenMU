# C3 F3 03 - CharacterInformation075 (by server)

## Is sent when

After the character was selected by the player and entered the game.

## Causes the following actions on the client side

The characters enters the game world.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   42   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | X |
| 5 | 1 | Byte |  | Y |
| 6 | 1 | Byte |  | MapId |
| 8 | 4 | IntegerLittleEndian |  | CurrentExperience |
| 12 | 4 | IntegerLittleEndian |  | ExperienceForNextLevel |
| 16 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 18 | 2 | ShortLittleEndian |  | Strength |
| 20 | 2 | ShortLittleEndian |  | Agility |
| 22 | 2 | ShortLittleEndian |  | Vitality |
| 24 | 2 | ShortLittleEndian |  | Energy |
| 26 | 2 | ShortLittleEndian |  | CurrentHealth |
| 28 | 2 | ShortLittleEndian |  | MaximumHealth |
| 30 | 2 | ShortLittleEndian |  | CurrentMana |
| 32 | 2 | ShortLittleEndian |  | MaximumMana |
| 36 | 4 | IntegerLittleEndian |  | Money |
| 40 | 1 | CharacterHeroState |  | HeroState |
| 41 | 1 | CharacterStatus |  | Status |

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