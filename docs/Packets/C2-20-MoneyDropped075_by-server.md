# C2 20 - MoneyDropped075 (by server)

## Is sent when

Money dropped on the ground.

## Causes the following actions on the client side

The client adds the money to the ground.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |   14   | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x20  | Packet header - packet type identifier |
| 4 | 1 | Byte | 1 | ItemCount |
| 5 | 2 | ShortBigEndian |  | Id |
| 5 << 7 | 1 bit | Boolean |  | IsFreshDrop; If this flag is set, the money is added to the map with an animation and sound. Otherwise it's just added like it was already on the ground before. |
| 7 | 1 | Byte |  | PositionX |
| 8 | 1 | Byte |  | PositionY |
| 9 | 4 bit | Byte | 15 | MoneyNumber |
| 9 | 4 bit | Byte | 14 | MoneyGroup |
| 10 | 4 | IntegerLittleEndian |  | Amount |