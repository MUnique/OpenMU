# C3 16 - ExperienceGained (by server)

## Is sent when

A player gained experience.

## Causes the following actions on the client side

The experience is added to the experience counter and bar.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x16  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | KilledObjectId |
| 5 | 2 | ShortBigEndian |  | AddedExperience |
| 7 | 2 | ShortBigEndian |  | DamageOfLastHit |