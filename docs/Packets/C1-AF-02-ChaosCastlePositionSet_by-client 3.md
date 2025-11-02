# C1 AF 02 - ChaosCastlePositionSet (by client)

## Is sent when

The game client noticed, that the coordinates of the player is not on the ground anymore. It requests to set the specified coordinates.

## Causes the following actions on the server side

The server sets the player on the new coordinates. Not handled on OpenMU.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | PositionX |
| 5 | 1 | Byte |  | PositionY |