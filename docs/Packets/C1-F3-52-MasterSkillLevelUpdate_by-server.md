# C1 F3 52 - MasterSkillLevelUpdate (by server)

## Is sent when

After a master skill level has been changed (usually increased).

## Causes the following actions on the client side

The level is updated in the master skill tree.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   28   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x52  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | Success |
| 6 | 2 | ShortLittleEndian |  | MasterLevelUpPoints |
| 8 | 1 | Byte |  | MasterSkillIndex; The index of the master skill on the clients master skill tree for the given character class. |
| 12 | 2 | ShortLittleEndian |  | MasterSkillNumber |
| 16 | 1 | Byte |  | Level |
| 20 | 4 | Float |  | DisplayValue |
| 24 | 4 | Float |  | DisplayValueOfNextLevel |