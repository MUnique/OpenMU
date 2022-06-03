# C1 9B - BloodCastleState (by server)

## Is sent when

The state of a blood castle event is about to change.

## Causes the following actions on the client side

The client side shows a message about the changing state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x9B  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | State |
| 4 | 2 | ShortLittleEndian |  | RemainSecond |
| 6 | 2 | ShortLittleEndian |  | MaxMonster |
| 8 | 2 | ShortLittleEndian |  | CurMonster |
| 10 | 2 | ShortLittleEndian |  | ItemOwner |
| 12 | 1 | Byte |  | ItemLevel |