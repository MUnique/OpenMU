# C1 F3 04 - RespawnAfterDeath095 (by server)

## Is sent when

The character respawned after death.

## Causes the following actions on the client side

The character respawns with the specified attributes at the specified map.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   22   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | PositionX |
| 5 | 1 | Byte |  | PositionY |
| 6 | 1 | Byte |  | MapNumber |
| 7 | 1 | Byte |  | Direction |
| 8 | 2 | ShortLittleEndian |  | CurrentHealth |
| 10 | 2 | ShortLittleEndian |  | CurrentMana |
| 12 | 2 | ShortLittleEndian |  | CurrentAbility |
| 14 | 4 | IntegerLittleEndian |  | Experience |
| 18 | 4 | IntegerLittleEndian |  | Money |