# C2 B2 1A - CastleSiegeMiniMapPlayerPositions (by server)

## Is sent when

The server sends the positions of all players in the castle siege mini map.

## Causes the following actions on the client side

The client updates the mini map with the player positions.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x1A  | Packet header - sub packet type identifier |
| 5 | 4 | IntegerBigEndian |  | PlayerCount |
| 9 | MiniMapPlayerPosition.Length * PlayerCount | Array of MiniMapPlayerPosition |  | Players |

### MiniMapPlayerPosition Structure

The position of one player on the mini map.

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | PositionX |
| 1 | 1 | Byte |  | PositionY |