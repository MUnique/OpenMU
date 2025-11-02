# C1 B2 06 - CastleSiegeDefenseRepairRequest (by client)

## Is sent when

The player opened a castle siege npc and requests to repair a gate or statue at a specific position (index)..

## Causes the following actions on the server side

The server returns a response.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | NpcNumber |
| 8 | 4 | IntegerLittleEndian |  | NpcIndex |