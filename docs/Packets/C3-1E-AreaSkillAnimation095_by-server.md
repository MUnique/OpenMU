# C3 1E - AreaSkillAnimation095 (by server)

## Is sent when

An object performs a skill which has effect on an area.

## Causes the following actions on the client side

The animation is shown on the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1E  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillId |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 1 | Byte |  | PointX |
| 7 | 1 | Byte |  | PointY |
| 8 | 1 | Byte |  | Rotation |