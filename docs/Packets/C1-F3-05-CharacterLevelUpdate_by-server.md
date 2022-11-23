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
| 4 | 2 | ShortLittleEndian |  | Level |
| 6 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 8 | 2 | ShortLittleEndian |  | MaximumHealth |
| 10 | 2 | ShortLittleEndian |  | MaximumMana |
| 12 | 2 | ShortLittleEndian |  | MaximumShield |
| 14 | 2 | ShortLittleEndian |  | MaximumAbility |
| 16 | 2 | ShortLittleEndian |  | FruitPoints |
| 18 | 2 | ShortLittleEndian |  | MaximumFruitPoints |
| 20 | 2 | ShortLittleEndian |  | NegativeFruitPoints |
| 22 | 2 | ShortLittleEndian |  | MaximumNegativeFruitPoints |