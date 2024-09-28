# C1 25 - AppearanceChangedExtended (by server)

## Is sent when

The appearance of a player changed, all surrounding players are informed about it.

## Causes the following actions on the client side

The appearance of the player is updated.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x25  | Packet header - packet type identifier |
| 3 | 2 | ShortLittleEndian |  | ChangedPlayerId |
| 5 | 4 bit | Byte |  | ItemSlot |
| 5 | 4 bit | Byte |  | ItemGroup |
| 6 | 2 | ShortLittleEndian |  | ItemNumber |
| 8 | 1 | Byte |  | ItemLevel |
| 9 << 0 | 1 bit | Boolean |  | IsExcellent |
| 9 << 1 | 1 bit | Boolean |  | IsAncient |
| 9 << 2 | 1 bit | Boolean |  | IsAncientSetComplete |