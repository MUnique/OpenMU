# C1 B2 1F - CastleSiegeHuntingZoneEntranceSetting (by client)

## Is sent when

A guild member of the castle owners wants to enter the hunting zone (e.g. Land of Trials).

## Causes the following actions on the server side

The server changes the entrance setting of the hunting zone.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x1F  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | IsPublic |