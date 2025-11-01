# C1 BF 02 - IllusionTempleSkillRequest (by client)

## Is sent when

The player is in the illusion temple event and wants to perform a special skill (210 - 213), Order of Protection, Restraint, Tracking or Weaken.

## Causes the following actions on the server side

The server checks if the player is inside the event etc. and performs the skills accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | SkillNumber |
| 6 | 1 | Byte |  | TargetObjectIndex |
| 7 | 1 | Byte |  | Distance |