# C1 F3 05 - CharacterLevelUpdateExtended (by server)

## Is sent when

After a character leveled up.

## Causes the following actions on the client side

Updates the level (and other related stats) in the game client and shows an effect.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   32   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | Level |
| 6 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 8 | 4 | IntegerLittleEndian |  | MaximumHealth |
| 12 | 4 | IntegerLittleEndian |  | MaximumMana |
| 16 | 4 | IntegerLittleEndian |  | MaximumShield |
| 20 | 4 | IntegerLittleEndian |  | MaximumAbility |
| 24 | 2 | ShortLittleEndian |  | FruitPoints |
| 26 | 2 | ShortLittleEndian |  | MaximumFruitPoints |
| 28 | 2 | ShortLittleEndian |  | NegativeFruitPoints |
| 30 | 2 | ShortLittleEndian |  | MaximumNegativeFruitPoints |