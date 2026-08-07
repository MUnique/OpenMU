# C1 D5 - WalkRequestGlobal (by client)

## Is sent when

A global-world player wants to walk from an absolute ushort coordinate.

## Causes the following actions on the server side

The player gets moved using global-world coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD5  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | SourceX |
| 5 | 2 | ShortBigEndian |  | SourceY |
| 7 | 4 bit | Byte |  | StepCount |
| 7 | 4 bit | Byte |  | TargetRotation |
| 8 |  | Binary |  | Directions |