# C1 2F - MoneyDroppedExtended (by server)

## Is sent when

Money dropped on the ground.

## Causes the following actions on the client side

The client adds the money to the ground.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x2F  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | IsFreshDrop; If this flag is set, the money is added to the map with an animation and sound. Otherwise, it's just added like it was already on the ground before. |
| 4 | 2 | ShortLittleEndian |  | Id |
| 6 | 1 | Byte |  | PositionX |
| 7 | 1 | Byte |  | PositionY |
| 8 | 4 | IntegerLittleEndian |  | Amount |