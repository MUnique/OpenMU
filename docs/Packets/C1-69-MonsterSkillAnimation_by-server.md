# C1 69 - MonsterSkillAnimation (by server)

## Is sent when

A monster performs a special skill, e.g. Selupan.

## Causes the following actions on the client side

The client shows the animation of the monster skill.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x69  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | SkillNumber; The number of the monster skill of the client, e.g. 34 to 42 for the skills of Selupan. |
| 6 | 2 | ShortLittleEndian |  | AttackerId; The id of the monster. The field is aligned to 2 bytes, because the client structure is not packed. |
| 8 | 2 | ShortLittleEndian |  | TargetId; The id of the target. The highest bit is set, if the skill has been applied successfully. |