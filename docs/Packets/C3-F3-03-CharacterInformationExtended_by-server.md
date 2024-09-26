# C3 F3 03 - CharacterInformationExtended (by server)

## Is sent when

After the character was selected by the player and entered the game.

## Causes the following actions on the client side

The characters enters the game world.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   96   | Packet header - length of the packet |
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
| 34 | 2 | ShortLittleEndian |  | Leadership |
| 36 | 4 | IntegerLittleEndian |  | CurrentHealth |
| 40 | 4 | IntegerLittleEndian |  | MaximumHealth |
| 44 | 4 | IntegerLittleEndian |  | CurrentMana |
| 48 | 4 | IntegerLittleEndian |  | MaximumMana |
| 52 | 4 | IntegerLittleEndian |  | CurrentShield |
| 56 | 4 | IntegerLittleEndian |  | MaximumShield |
| 60 | 4 | IntegerLittleEndian |  | CurrentAbility |
| 64 | 4 | IntegerLittleEndian |  | MaximumAbility |
| 68 | 4 | IntegerLittleEndian |  | Money |
| 72 | 1 | CharacterHeroState |  | HeroState |
| 73 | 1 | CharacterStatus |  | Status |
| 74 | 2 | ShortLittleEndian |  | UsedFruitPoints |
| 76 | 2 | ShortLittleEndian |  | MaxFruitPoints |
| 78 | 2 | ShortLittleEndian |  | UsedNegativeFruitPoints |
| 80 | 2 | ShortLittleEndian |  | MaxNegativeFruitPoints |
| 82 | 2 | ShortLittleEndian |  | AttackSpeed |
| 84 | 2 | ShortLittleEndian |  | MagicSpeed |
| 86 | 2 | ShortLittleEndian |  | MaximumAttackSpeed |
| 88 | 1 | Byte |  | InventoryExtensions |

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