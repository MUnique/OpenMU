# C1 B2 07 - CastleSiegeDefenseUpgradeRequest (by client)

## Is sent when

The player opened a castle siege npc and requests to upgrade a gate or statue at a specific position (index)..

## Causes the following actions on the server side

The server returns a response.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   20   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | NpcNumber |
| 8 | 4 | IntegerLittleEndian |  | NpcIndex |
| 12 | 4 | IntegerLittleEndian |  | NpcUpgradeType |
| 16 | 4 | IntegerLittleEndian |  | NpcUpgradeValue |