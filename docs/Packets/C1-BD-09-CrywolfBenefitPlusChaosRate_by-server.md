# C1 BD 09 - CrywolfBenefitPlusChaosRate (by server)

## Is sent when

The player opened a crafting dialog and requested the chaos rate benefit of the crywolf event.

## Causes the following actions on the client side

The client shows the additional success rate of the crafting.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | PlusChaosRate; The additional success rate in percent. |