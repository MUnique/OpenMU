# C3 4A - RageAttackRequest (by client)

## Is sent when

A player performs a skill with a target, e.g. attacking or buffing.

## Causes the following actions on the server side

Damage is calculated and the target is hit, if the attack was successful. A response is sent back with the caused damage, and all surrounding players get an animation message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x4A  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | SkillId |
| 6 | 2 | ShortBigEndian |  | TargetId |