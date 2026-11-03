# C1 F3 13 - PlayerEquipment (by server)

## Is sent when

After the equipment of a player in the viewport changed.

## Causes the following actions on the client side

The client updates the appearance of the player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   24   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x13  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 18 | Binary |  | Appearance; Same encoding as the appearance in the character list; the first (class) byte is not evaluated. |