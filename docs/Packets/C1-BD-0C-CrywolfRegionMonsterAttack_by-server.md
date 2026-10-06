# C1 BD 0C - CrywolfRegionMonsterAttack (by server)

## Is sent when

A ballista of the crywolf event attacks an area.

## Causes the following actions on the client side

The client shows an arrow which hits the target point. The direction of the arrow depends on the x coordinate of the ballista.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0C  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | MonsterNumber; The number of the monster. The client ignores it. |
| 6 | 1 | Byte |  | SourceX; The x coordinate of the ballista. |
| 7 | 1 | Byte |  | SourceY; The y coordinate of the ballista. The client ignores it. |
| 8 | 1 | Byte |  | TargetX; The x coordinate of the target point. |
| 9 | 1 | Byte |  | TargetY; The y coordinate of the target point. |