# C1 F3 32 - BaseStatsExtended (by server)

## Is sent when

Setting the base stats of a character, e.g. set stats command or after a reset.

## Causes the following actions on the client side

The values are updated on the game client user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   24   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x32  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Strength |
| 8 | 4 | IntegerLittleEndian |  | Agility |
| 12 | 4 | IntegerLittleEndian |  | Vitality |
| 16 | 4 | IntegerLittleEndian |  | Energy |
| 20 | 4 | IntegerLittleEndian |  | Command |