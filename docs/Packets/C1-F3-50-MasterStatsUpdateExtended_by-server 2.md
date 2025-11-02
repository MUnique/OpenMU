# C1 F3 50 - MasterStatsUpdateExtended (by server)

## Is sent when

After entering the game with a master class character.

## Causes the following actions on the client side

The master related data is available.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   40   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x50  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | MasterLevel |
| 6 | 8 | LongBigEndian |  | MasterExperience |
| 14 | 8 | LongBigEndian |  | MasterExperienceOfNextLevel |
| 22 | 2 | ShortLittleEndian |  | MasterLevelUpPoints |
| 24 | 4 | IntegerLittleEndian |  | MaximumHealth |
| 28 | 4 | IntegerLittleEndian |  | MaximumMana |
| 32 | 4 | IntegerLittleEndian |  | MaximumShield |
| 36 | 4 | IntegerLittleEndian |  | MaximumAbility |