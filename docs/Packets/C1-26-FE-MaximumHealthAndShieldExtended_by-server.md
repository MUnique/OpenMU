# C1 26 FE - MaximumHealthAndShieldExtended (by server)

## Is sent when

When the maximum health changed, e.g. by adding stat points or changed items.

## Causes the following actions on the client side

The health and shield bar is updated on the game client user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x26  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0xFE  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Health |
| 8 | 4 | IntegerLittleEndian |  | Shield |