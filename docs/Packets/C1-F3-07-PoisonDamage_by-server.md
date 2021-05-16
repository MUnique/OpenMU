# C1 F3 07 - PoisonDamage (by server)

## Is sent when

The character got damaged by being poisoned.

## Causes the following actions on the client side

Shows poison damage, colors the health bar green.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | HealthDamage |
| 6 | 2 | ShortBigEndian |  | ShieldDamage |