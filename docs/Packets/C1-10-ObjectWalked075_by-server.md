# C1 10 - ObjectWalked075 (by server)

## Is sent when

An object in the observed scope (including the own player) walked to another position.

## Causes the following actions on the client side

The object is animated to walk to the new position.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x10  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 1 | Byte |  | TargetX |
| 6 | 1 | Byte |  | TargetY |
| 7 | 4 bit | Byte |  | TargetRotation |