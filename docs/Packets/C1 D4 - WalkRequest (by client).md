# C1 D4 - WalkRequest (by client)

## Is sent when

A player wants to walk on the game map.

## Causes the following actions on the server side

The player gets moved on the map, visible for other surrounding players.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD4  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | SourceX |
| 4 | 1 | Byte |  | SourceY |
| 5 |  | Binary |  | Directions; The directions of the walking path. The target is calculated by taking the source coordinates and applying the directions to it. |