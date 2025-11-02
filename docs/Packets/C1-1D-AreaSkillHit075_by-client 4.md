# C1 1D - AreaSkillHit075 (by client)

## Is sent when

An area skill was performed and the client decided to hit one or more targets.

## Causes the following actions on the server side

The server is calculating the damage and applying it to the targets. The attacker gets a response back with the caused damage.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x1D  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SkillIndex; The index of the skill in the skill list. |
| 4 | 1 | Byte |  | TargetX |
| 5 | 1 | Byte |  | TargetY |
| 6 | 1 | Byte |  | TargetCount; The number of targets which are hit. |
| 7 | TargetData.Length * TargetCount | Array of TargetData |  | Targets |

### TargetData Structure

Contains the data of the target

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | TargetId |