# C1 BF 02 - IllusionTempleSkillUsageResult (by server)

## Is sent when

A player requested to use a specific skill in the illusion temple event.

## Causes the following actions on the client side

The client shows the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   11   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 2 | ShortBigEndian |  | SkillNumber |
| 7 | 2 | ShortLittleEndian |  | SourceObjectId |
| 9 | 2 | ShortLittleEndian |  | TargetObjectId |