# C2 45 - AddTransformedCharacterToScopeExtended (by server)

## Is sent when

The player wears a monster transformation ring (extended client).

## Causes the following actions on the client side

The character appears as monster, defined by the Skin property.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x45  | Packet header - packet type identifier |
| 4 | 1 | Byte | 1 | CharacterCount; The number of characters in this packet. This packet contains only one character, because the size of the appearance data depends on the used appearance serializer. |
| 5 | 2 | ShortBigEndian |  | Id |
| 7 | 1 | Byte |  | CurrentPositionX |
| 8 | 1 | Byte |  | CurrentPositionY |
| 9 | 2 | ShortBigEndian |  | Skin |
| 11 | 10 | String |  | Name |
| 21 | 1 | Byte |  | TargetPositionX |
| 22 | 1 | Byte |  | TargetPositionY |
| 23 | 4 bit | Byte |  | Rotation |
| 23 << 0 | 4 bit | CharacterHeroState |  | HeroState |
| 24 |  | Binary |  | AppearanceAndEffects; The appearance data, followed by the number of effects and the effect ids. |

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