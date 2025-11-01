# C1 BF 07 - IllusionTempleSkillEnded (by server)

## Is sent when

?

## Causes the following actions on the client side

The client shows the skill points.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | SkillNumber |
| 6 | 2 | ShortLittleEndian |  | ObjectIndex |