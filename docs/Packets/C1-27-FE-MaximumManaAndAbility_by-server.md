# C1 27 FE - MaximumManaAndAbility (by server)

## Is sent when

The maximum available mana or ability has changed, e.g. by adding stat points.

## Causes the following actions on the client side

The mana and ability bar is updated on the game client user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x27  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0xFE  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | Mana |
| 6 | 2 | ShortBigEndian |  | Ability |