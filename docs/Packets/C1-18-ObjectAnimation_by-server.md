# C1 18 - ObjectAnimation (by server)

## Is sent when

An object performs an animation.

## Causes the following actions on the client side

The animation is shown for the specified object.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x18  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 1 | Byte |  | Direction |
| 6 | 1 | Byte |  | Animation |
| 7 | 2 | ShortBigEndian |  | TargetId |