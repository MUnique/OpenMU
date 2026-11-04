# C2 B3 - CastleSiegeNpcList (by server)

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
| 4 | 1 | Byte |  | Result |
| 5 | 4 | IntegerLittleEndian |  | NpcCount |
| 9 | CastleSiegeNpcInfo.Length * NpcCount | Array of CastleSiegeNpcInfo |  | NpcList |

### CastleSiegeNpcInfo Structure

Information about one castle siege NPC (gate or statue).

Length: 27 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | NpcNumber |
| 4 | 4 | IntegerLittleEndian |  | NpcIndex |
| 8 | 4 | IntegerLittleEndian |  | DefenseUpgradeLevel |
| 12 | 4 | IntegerLittleEndian |  | RegenerationLevel |
| 16 | 4 | IntegerLittleEndian |  | MaxHp |
| 20 | 4 | IntegerLittleEndian |  | CurrentHp |
| 24 | 1 | Byte |  | PositionX |
| 25 | 1 | Byte |  | PositionY |
| 26 | 1 | Boolean |  | IsAlive |