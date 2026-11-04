# C1 4B - RageAttackRangeRequest (by client)

## Is sent when

A player (rage fighter) performs the dark side skill on a target.

## Causes the following actions on the server side

The targets (up to 5) are determined and sent back to the player with the RageAttackRangeResponse.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x4B  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | SkillId |
| 5 | 2 | ShortBigEndian |  | TargetId |