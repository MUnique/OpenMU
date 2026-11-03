# C1 1A - SkillTargetsAnimation (by server)

## Is sent when

After a player used a skill which hits several targets at a position.

## Causes the following actions on the client side

The client plays the skill animation of the player and the hit animation for each target.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1A  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | PlayerId |
| 5 | 2 | ShortBigEndian |  | SkillId |
| 7 | 1 | Byte |  | PositionX |
| 8 | 1 | Byte |  | PositionY |
| 9 | 1 | Byte |  | TargetCount |
| 10 | SkillTarget.Length * TargetCount | Array of SkillTarget |  | Targets |

### SkillTarget Structure

The id of a target which was hit by the skill.

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | TargetId |