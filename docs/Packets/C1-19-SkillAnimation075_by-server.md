# C1 19 - SkillAnimation075 (by server)

## Is sent when

An object performs a skill which is directly targeted to another object.

## Causes the following actions on the client side

The animation is shown on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x19  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillId |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 2 | ShortBigEndian |  | TargetId |
| 6 << 7 | 1 bit | Boolean |  | EffectApplied |