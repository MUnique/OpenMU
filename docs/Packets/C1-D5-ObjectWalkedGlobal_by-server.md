# C1 D5 - ObjectWalkedGlobal (by server)

## Is sent when

An object in the global-world scope walked to another absolute position.

## Causes the following actions on the client side

The object is animated to walk using global coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD5  | Packet header - packet type identifier |
| 2 | 1 | Byte |  | HeaderCode |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 2 | ShortBigEndian |  | SourceX |
| 7 | 2 | ShortBigEndian |  | SourceY |
| 9 | 2 | ShortBigEndian |  | TargetX |
| 11 | 2 | ShortBigEndian |  | TargetY |
| 13 | 4 bit | Byte |  | TargetRotation |
| 13 | 4 bit | Byte |  | StepCount |
| 14 |  | Binary |  | StepData |