# C1 F3 11 - SkillListUpdate (by server)

## Is sent when

Usually, when the player entered the game with a character. When skills get added or removed, this message is sent as well, but with a misleading count.

## Causes the following actions on the client side

The skill list gets initialized.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x11  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Count; Mixed usage: Skill list count (when list). 0xFE when adding a skill, 0xFF when removing a Skill. |
| 6 | SkillEntry.Length * Count | Array of SkillEntry |  | Skills |

### SkillEntry Structure

Structure for a skill entry of the skill list.

Length: 4 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | SkillIndex |
| 1 | 2 | ShortLittleEndian |  | SkillNumber |
| 3 | 1 | Byte |  | SkillLevel |