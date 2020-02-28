# C1 F3 05 - CharacterLevelUpdate (by server)

## Is sent when

After a character leveled up.

## Causes the following actions on the client side

Updates the level (and other related stats) in the game client and shows an effect.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   24   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | Level |
| 6 | 2 | ShortBigEndian |  | LevelUpPoints |
| 8 | 2 | ShortBigEndian |  | MaximumHealth |
| 10 | 2 | ShortBigEndian |  | MaximumMana |
| 12 | 2 | ShortBigEndian |  | MaximumShield |
| 14 | 2 | ShortBigEndian |  | MaximumAbility |
| 16 | 2 | ShortBigEndian |  | FruitPoints |
| 18 | 2 | ShortBigEndian |  | MaximumFruitPoints |
| 20 | 2 | ShortBigEndian |  | NegativeFruitPoints |
| 22 | 2 | ShortBigEndian |  | MaximumNegativeFruitPoints |