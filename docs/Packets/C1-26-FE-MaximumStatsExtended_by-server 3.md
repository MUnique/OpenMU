# C1 26 FE - MaximumStatsExtended (by server)

## Is sent when

When the maximum stats, like health, shield, mana or attack speed changed on the server side, e.g. by adding stat points or changed items.

## Causes the following actions on the client side

The values are updated on the game client user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   20   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x26  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0xFE  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Health |
| 8 | 4 | IntegerLittleEndian |  | Shield |
| 12 | 4 | IntegerLittleEndian |  | Mana |
| 16 | 4 | IntegerLittleEndian |  | Ability |