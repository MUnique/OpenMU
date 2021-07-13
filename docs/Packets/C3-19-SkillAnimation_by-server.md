# C3 19 - SkillAnimation (by server)

## Is sent when

An object performs a skill which is directly targeted to another object.

## Causes the following actions on the client side

The animation is shown on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x19  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | SkillId |
| 5 | 2 | ShortBigEndian |  | PlayerId |
| 7 | 2 | ShortBigEndian |  | TargetId |