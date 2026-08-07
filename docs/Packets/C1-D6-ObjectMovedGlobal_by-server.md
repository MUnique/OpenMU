# C1 D6 - ObjectMovedGlobal (by server)

## Is sent when

An object in the global-world scope moved instantly.

## Causes the following actions on the client side

The client updates the object position using absolute ushort coordinates.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD6  | Packet header - packet type identifier |
| 2 | 1 | Byte |  | HeaderCode |
| 3 | 2 | ShortBigEndian |  | ObjectId |
| 5 | 2 | ShortBigEndian |  | PositionX |
| 7 | 2 | ShortBigEndian |  | PositionY |