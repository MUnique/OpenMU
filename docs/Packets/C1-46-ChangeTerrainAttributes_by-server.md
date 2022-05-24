# C1 46 - ChangeTerrainAttributes (by server)

## Is sent when

The server wants to alter the terrain attributes of a map at runtime.

## Causes the following actions on the client side

The client updates the terrain attributes on its side.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x46  | Packet header - packet type identifier |
| 3 | 1 | Boolean | false | Type |
| 4 | 1 | TerrainAttributeType |  | Attribute |
| 5 | 1 | Boolean |  | SetAttribute |
| 6 | 1 | Byte |  | AreaCount |
| 7 | TerrainArea.Length * AreaCount | Array of TerrainArea |  | Areas |

### TerrainArea Structure

Defines the area which should be changed.

Length: 4 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | StartX |
| 1 | 1 | Byte |  | StartY |
| 2 | 1 | Byte |  | EndX |
| 3 | 1 | Byte |  | EndY |

### TerrainAttributeType Enum

Defines the attribute which should be set/unset.

| Value | Name | Description |
|-------|------|-------------|
| 1 | Safezone | The coordinate is a safezone. |
| 4 | Blocked | The coordinate is blocked and can't be passed by a character. |
| 4 | BlockedByTrap | The coordinate is blocked by a trap and can't be passed by a character. |
| 16 | BlockedByImperialGate | The coordinate is blocked by a imperial battle gate and can't be passed by a character. |