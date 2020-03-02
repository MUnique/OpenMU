# C1 F3 11 - SkillAdded075 (by server)

## Is sent when

After a skill got added to the skill list, e.g. by equipping an item or learning a skill.

## Causes the following actions on the client side

The skill is added to the skill list on client side.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x11  | Packet header - sub packet type identifier |
| 4 | 1 | Byte | 0xFE | Flag |
| 5 | 1 | Byte |  | SkillIndex |
| 6 | 2 | ShortBigEndian |  | SkillNumberAndLevel |