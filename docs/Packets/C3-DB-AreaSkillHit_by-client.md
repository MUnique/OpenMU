# C3 DB - AreaSkillHit (by client)

## Is sent when

An area skill was performed and the client decided to hit a target.

## Causes the following actions on the server side

The server is calculating the damage and applying it to the target. The attacker gets a response back with the caused damage.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xDB  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | SkillId |
| 5 | 1 | Byte |  | TargetX |
| 6 | 1 | Byte |  | TargetY |
| 7 | 1 | Byte |  | HitCounter; A sequential hit counter which should prevent that hits are sent multiple times. |
| 9 | 2 | ShortBigEndian |  | TargetId |
| 11 | 1 | Byte |  | AnimationCounter; A sequential animation counter which acts as a reference to the previously sent Area Skill Animation packet. |