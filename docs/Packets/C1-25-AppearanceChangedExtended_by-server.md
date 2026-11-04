# C1 25 - AppearanceChangedExtended (by server)

## Is sent when

The appearance of a player changed, all surrounding players are informed about it.

## Causes the following actions on the client side

The appearance of the player is updated.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   14   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x25  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | ChangedPlayerId |
| 6 | 1 | Byte |  | ItemSlot |
| 7 | 1 | Byte |  | ItemGroup |
| 8 | 2 | ShortLittleEndian |  | ItemNumber |
| 10 | 1 | Byte |  | ItemLevel |
| 11 | 1 | Byte |  | ExcellentFlags |
| 12 | 1 | Byte |  | AncientDiscriminator |
| 13 | 1 | Boolean |  | IsAncientSetComplete |