# C1 8E 02 - WarpCommandRequest (by client)

## Is sent when

A player selected to warp by selecting an entry in the warp list (configured in game client files).

## Causes the following actions on the server side

If the player has enough money and is allowed to enter the map, it's getting moved to there.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x8E  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 8 | 2 | ShortLittleEndian |  | WarpInfoIndex; The index of the entry in the warp list. |