# C1 F3 50 - MasterStatsUpdate (by server)

## Is sent when

After entering the game with a master class character.

## Causes the following actions on the client side

The master related data is available.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   32   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x50  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | MasterLevel |
| 6 | 8 | LongBigEndian |  | MasterExperience |
| 14 | 8 | LongBigEndian |  | MasterExperienceOfNextLevel |
| 22 | 2 | ShortLittleEndian |  | MasterLevelUpPoints |
| 24 | 2 | ShortLittleEndian |  | MaximumHealth |
| 26 | 2 | ShortLittleEndian |  | MaximumMana |
| 28 | 2 | ShortLittleEndian |  | MaximumShield |
| 30 | 2 | ShortLittleEndian |  | MaximumAbility |