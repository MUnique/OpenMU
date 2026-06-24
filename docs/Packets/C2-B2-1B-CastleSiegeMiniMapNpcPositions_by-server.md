# C2 B2 1B - CastleSiegeMiniMapNpcPositions (by server)

## Is sent when

The server sends the positions of all siege NPCs in the castle siege mini map.

## Causes the following actions on the client side

The client updates the mini map with the NPC positions.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x1B  | Packet header - sub packet type identifier |
| 5 | 4 | IntegerBigEndian |  | NpcCount |
| 9 | MiniMapNpcPosition.Length * NpcCount | Array of MiniMapNpcPosition |  | Npcs |

### MiniMapNpcPosition Structure

The position of one NPC on the mini map.

Length: 3 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | NpcType |
| 1 | 1 | Byte |  | PositionX |
| 2 | 1 | Byte |  | PositionY |