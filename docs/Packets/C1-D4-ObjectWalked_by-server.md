# C1 D4 - ObjectWalked (by server)

## Is sent when

An object in the observed scope (including the own player) walked to another position.

## Causes the following actions on the client side

The object is animated to walk to the new position.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD4  | Packet header - packet type identifier |
| 2 | 1 | Byte |  | HeaderCode |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 1 | Byte |  | TargetX |
| 6 | 1 | Byte |  | TargetY |
| 7 | 4 bit | Byte |  | TargetRotation |
| 7 | 4 bit | Byte |  | StepCount |
| 8 |  | Binary |  | StepData |