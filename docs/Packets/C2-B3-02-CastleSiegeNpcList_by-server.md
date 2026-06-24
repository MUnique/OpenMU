# C2 B3 02 - CastleSiegeNpcList (by server)

## Is sent when

After the guild master requested the list of all castle siege statues and gates.

## Causes the following actions on the client side

The client shows the list of castle siege NPCs with their current status.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xB3  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | Result |
| 6 | 4 | IntegerBigEndian |  | NpcCount |
| 10 | CastleSiegeNpcInfo.Length * NpcCount | Array of CastleSiegeNpcInfo |  | NpcList |

### CastleSiegeNpcInfo Structure

Information about one castle siege NPC (gate or statue).

Length: 21 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | NpcNumber |
| 4 | 4 | IntegerLittleEndian |  | NpcIndex |
| 8 | 1 | Byte |  | DefenseUpgradeLevel |
| 9 | 1 | Byte |  | RegenerationLevel |
| 10 | 4 | IntegerLittleEndian |  | MaxHp |
| 14 | 4 | IntegerLittleEndian |  | CurrentHp |
| 18 | 1 | Byte |  | PositionX |
| 19 | 1 | Byte |  | PositionY |
| 20 | 1 | Boolean |  | IsAlive |