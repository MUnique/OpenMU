# C3 F3 61 - CharacterInformationGlobal (by server)

## Is sent when

After a character with global coordinates was selected and entered the game.

## Causes the following actions on the client side

The character enters the global game world.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   94   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x61  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | X |
| 6 | 2 | ShortLittleEndian |  | Y |
| 8 | 2 | ShortLittleEndian |  | MapId |
| 10 | 8 | LongBigEndian |  | CurrentExperience |
| 18 | 8 | LongBigEndian |  | ExperienceForNextLevel |
| 26 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 28 | 2 | ShortLittleEndian |  | Strength |
| 30 | 2 | ShortLittleEndian |  | Agility |
| 32 | 2 | ShortLittleEndian |  | Vitality |
| 34 | 2 | ShortLittleEndian |  | Energy |
| 36 | 2 | ShortLittleEndian |  | Leadership |
| 38 | 4 | IntegerLittleEndian |  | CurrentHealth |
| 42 | 4 | IntegerLittleEndian |  | MaximumHealth |
| 46 | 4 | IntegerLittleEndian |  | CurrentMana |
| 50 | 4 | IntegerLittleEndian |  | MaximumMana |
| 54 | 4 | IntegerLittleEndian |  | CurrentShield |
| 58 | 4 | IntegerLittleEndian |  | MaximumShield |
| 62 | 4 | IntegerLittleEndian |  | CurrentAbility |
| 66 | 4 | IntegerLittleEndian |  | MaximumAbility |
| 70 | 4 | IntegerLittleEndian |  | Money |
| 74 | 1 | CharacterHeroState |  | HeroState |
| 75 | 1 | CharacterStatus |  | Status |
| 76 | 2 | ShortLittleEndian |  | UsedFruitPoints |
| 78 | 2 | ShortLittleEndian |  | MaxFruitPoints |
| 80 | 2 | ShortLittleEndian |  | UsedNegativeFruitPoints |
| 82 | 2 | ShortLittleEndian |  | MaxNegativeFruitPoints |
| 84 | 2 | ShortLittleEndian |  | AttackSpeed |
| 86 | 2 | ShortLittleEndian |  | MagicSpeed |
| 88 | 2 | ShortLittleEndian |  | MaximumAttackSpeed |
| 90 | 1 | Byte |  | InventoryExtensions |
| 92 | 2 | ShortLittleEndian |  | Resets |

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