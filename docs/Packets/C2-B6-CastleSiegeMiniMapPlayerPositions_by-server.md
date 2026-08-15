# C2 B6 - CastleSiegeMiniMapPlayerPositions (by server)

## Is sent when

The server sends same-side player positions to a Castle Siege mini-map requester.

## Causes the following actions on the client side

The client updates the mini map with the player positions.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xB6  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | PlayerCount |
| 8 | MiniMapPlayerPosition.Length * PlayerCount | Array of MiniMapPlayerPosition |  | Players |

### MiniMapPlayerPosition Structure

The position of one player on the mini map.

Length: 2 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | PositionX |
| 1 | 1 | Byte |  | PositionY |