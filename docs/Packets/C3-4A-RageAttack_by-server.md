# C3 4A - RageAttack (by server)

## Is sent when

A player (rage fighter) performs the dark side skill on a target and sent a RageAttackRangeRequest.

## Causes the following actions on the client side

The targets are attacked with visual effects.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x4A  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | SkillId |
| 5 | 2 | ShortBigEndian |  | SourceId |
| 7 | 2 | ShortBigEndian |  | TargetId |