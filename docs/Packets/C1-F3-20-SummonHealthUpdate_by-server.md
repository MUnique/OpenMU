# C1 F3 20 - SummonHealthUpdate (by server)

## Is sent when

When health of a summoned monster (Elf Skill) changed.

## Causes the following actions on the client side

The health is updated on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x20  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | HealthPercent |