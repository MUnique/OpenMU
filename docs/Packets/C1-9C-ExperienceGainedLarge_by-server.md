# C1 9C - ExperienceGainedLarge (by server)

## Is sent when

After a player gained experience which does not fit into 16 bits.

## Causes the following actions on the client side

The client adds the experience and shows the damage of the last hit.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x9C  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | KilledObjectId |
| 6 | 2 | ShortLittleEndian |  | AddedExperienceHigh; The upper 16 bits of the added experience. |
| 8 | 2 | ShortLittleEndian |  | AddedExperienceLow; The lower 16 bits of the added experience. |
| 10 | 2 | ShortBigEndian |  | DamageOfLastHit |