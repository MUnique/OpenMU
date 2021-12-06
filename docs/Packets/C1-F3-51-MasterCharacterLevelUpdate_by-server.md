# C1 F3 51 - MasterCharacterLevelUpdate (by server)

## Is sent when

After a master character leveled up.

## Causes the following actions on the client side

Updates the master level (and other related stats) in the game client and shows an effect.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   20   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x51  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | MasterLevel |
| 6 | 2 | ShortLittleEndian |  | GainedMasterPoints |
| 8 | 2 | ShortLittleEndian |  | CurrentMasterPoints |
| 10 | 2 | ShortLittleEndian |  | MaximumMasterPoints |
| 12 | 2 | ShortLittleEndian |  | MaximumHealth |
| 14 | 2 | ShortLittleEndian |  | MaximumMana |
| 16 | 2 | ShortLittleEndian |  | MaximumShield |
| 18 | 2 | ShortLittleEndian |  | MaximumAbility |