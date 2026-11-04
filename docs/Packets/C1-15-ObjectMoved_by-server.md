# C1 15 - ObjectMoved (by server)

## Is sent when

An object in the observed scope (including the own player) moved instantly.

## Causes the following actions on the client side

The position of the object is updated on client side.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x15  | Packet header - packet type identifier |
| 2 | 1 | Byte |  | HeaderCode |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 1 | Byte |  | PositionX |
| 6 | 1 | Byte |  | PositionY |