# C2 BB - CastleSiegeMiniMapNpcPositions (by server)

## Is sent when

The server sends alive gate and Guardian Statue positions to a Castle Siege mini-map requester.

## Causes the following actions on the client side

The client updates the mini map with the siege-NPC positions.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xBB  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | NpcCount |
| 5 | MiniMapNpcPosition.Length * NpcCount | Array of MiniMapNpcPosition |  | Npcs |

### MiniMapNpcPosition Structure

The position of one siege NPC on the mini map.

Length: 3 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | NpcType; 0 = gate, 1 = Guardian Statue |
| 1 | 1 | Byte |  | PositionX |
| 2 | 1 | Byte |  | PositionY |