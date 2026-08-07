# C1 F3 64 - RespawnAfterDeathGlobal (by server)

## Is sent when

The character respawned at a global coordinate.

## Causes the following actions on the client side

The character respawns with the specified attributes.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   31   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x64  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | PositionX |
| 6 | 2 | ShortLittleEndian |  | PositionY |
| 8 | 2 | ShortLittleEndian |  | MapNumber |
| 10 | 1 | Byte |  | Direction |
| 11 | 4 | IntegerLittleEndian |  | CurrentHealth |
| 15 | 4 | IntegerLittleEndian |  | CurrentMana |
| 19 | 4 | IntegerLittleEndian |  | CurrentShield |
| 23 | 4 | IntegerLittleEndian |  | CurrentAbility |
| 27 | 4 | IntegerLittleEndian |  | Money |