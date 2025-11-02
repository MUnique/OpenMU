# C1 B9 05 - CastleSiegeHuntingZoneEnterRequest (by client)

## Is sent when

A guild member of the castle owners wants to enter the hunting zone (e.g. Land of Trials).

## Causes the following actions on the server side

The server takes the entrance money, puts it into the tax wallet and warps the player to the hunting zone.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB9  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Money |