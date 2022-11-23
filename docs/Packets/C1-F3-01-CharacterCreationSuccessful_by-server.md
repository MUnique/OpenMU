# C1 F3 01 - CharacterCreationSuccessful (by server)

## Is sent when

After the server successfully processed a character creation request.

## Causes the following actions on the client side

The new character is shown in the character list

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   42   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean | true | Success |
| 5 | 10 | String |  | CharacterName |
| 15 | 1 | Byte |  | CharacterSlot |
| 16 | 2 | ShortLittleEndian |  | Level |
| 18 << 3 | 1 | CharacterClassNumber |  | Class |
| 19 | 1 | Byte |  | CharacterStatus |
| 20 |  | Binary |  | PreviewData |

### CharacterClassNumber Enum

Defines the values which are used for the character classes on client side.

| Value | Name | Description |
|-------|------|-------------|
| 0 | DarkWizard | Value for the dark wizard character class. |
| 2 | SoulMaster | Value for the soul master character class. |
| 3 | GrandMaster | Value for the grand master character class. |
| 4 | DarkKnight | Value for the dark knight character class. |
| 6 | BladeKnight | Value for the blade knight character class. |
| 7 | BladeMaster | Value for the blade master character class. |
| 8 | FairyElf | Value for the fairy elf character class. |
| 10 | MuseElf | Value for the muse elf character class. |
| 11 | HighElf | Value for the high elf character class. |
| 12 | MagicGladiator | Value for the magic gladiator character class. |
| 13 | DuelMaster | Value for the duel master character class. |
| 16 | DarkLord | Value for the dark lord character class. |
| 17 | LordEmperor | Value for the lord emperor character class. |
| 20 | Summoner | Value for the summoner character class. |
| 22 | BloodySummoner | Value for the bloody summoner character class. |
| 23 | DimensionMaster | Value for the dimension master character class. |
| 24 | RageFighter | Value for the rage fighter character class. |
| 25 | FistMaster | Value for the fist master character class. |