# C3 F3 03 - CharacterInformation (by server)

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
| 6 | 2 | ShortLittleEndian |  | MapId |
| 8 | 8 | LongBigEndian |  | CurrentExperience |
| 16 | 8 | LongBigEndian |  | ExperienceForNextLevel |
| 24 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 26 | 2 | ShortLittleEndian |  | Strength |
| 28 | 2 | ShortLittleEndian |  | Agility |
| 30 | 2 | ShortLittleEndian |  | Vitality |
| 32 | 2 | ShortLittleEndian |  | Energy |
| 34 | 2 | ShortLittleEndian |  | CurrentHealth |
| 36 | 2 | ShortLittleEndian |  | MaximumHealth |
| 38 | 2 | ShortLittleEndian |  | CurrentMana |
| 40 | 2 | ShortLittleEndian |  | MaximumMana |
| 42 | 2 | ShortLittleEndian |  | CurrentShield |
| 44 | 2 | ShortLittleEndian |  | MaximumShield |
| 46 | 2 | ShortLittleEndian |  | CurrentAbility |
| 48 | 2 | ShortLittleEndian |  | MaximumAbility |
| 52 | 4 | IntegerLittleEndian |  | Money |
| 56 | 1 | CharacterHeroState |  | HeroState |
| 57 | 1 | CharacterStatus |  | Status |
| 58 | 2 | ShortLittleEndian |  | UsedFruitPoints |
| 60 | 2 | ShortLittleEndian |  | MaxFruitPoints |
| 62 | 2 | ShortLittleEndian |  | Leadership |
| 64 | 2 | ShortLittleEndian |  | UsedNegativeFruitPoints |
| 66 | 2 | ShortLittleEndian |  | MaxNegativeFruitPoints |
| 68 | 1 | Byte |  | InventoryExtensions |

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