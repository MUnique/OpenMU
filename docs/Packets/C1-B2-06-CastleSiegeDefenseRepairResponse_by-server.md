# C1 B2 06 - CastleSiegeDefenseRepairResponse (by server)

## Is sent when

After the player requested to repair a castle siege defense structure (gate or statue).

## Causes the following actions on the client side

The client shows the result of the repair request.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 4 | IntegerLittleEndian |  | NpcNumber |
| 9 | 4 | IntegerLittleEndian |  | NpcIndex |