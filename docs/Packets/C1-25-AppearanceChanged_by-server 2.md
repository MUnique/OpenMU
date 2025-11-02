# C1 25 - AppearanceChanged (by server)

## Is sent when

The appearance of a player changed, all surrounding players are informed about it.

## Causes the following actions on the client side

The appearance of the player is updated.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x25  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | ChangedPlayerId |
| 5 |  | Binary |  | ItemData |