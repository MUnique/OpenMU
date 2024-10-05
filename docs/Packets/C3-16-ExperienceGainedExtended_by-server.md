# C3 16 - ExperienceGainedExtended (by server)

## Is sent when

A player gained experience.

## Causes the following actions on the client side

The experience is added to the experience counter and bar.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x16  | Packet header - packet type identifier |
| 3 | 1 | AddResult |  | Type |
| 4 | 4 | IntegerLittleEndian |  | AddedExperience |
| 8 | 4 | IntegerLittleEndian |  | DamageOfLastHit |
| 12 | 2 | ShortLittleEndian |  | KilledObjectId |
| 14 | 2 | ShortLittleEndian |  | KillerObjectId |

### AddResult Enum

Defines the result and type of experience which is added.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Undefined, no experience is added. |
| 1 | Normal | The normal experience is added. |
| 2 | Master | The master experience is added. |
| 16 | MaxLevelReached | The maximum level has been reached, no experience is added. |
| 32 | MaxMasterLevelReached | The maximum master level has been reached, no master experience is added. |
| 33 | MonsterLevelTooLowForMasterExperience | The monster level is too low for master experience, no master experience is added. |