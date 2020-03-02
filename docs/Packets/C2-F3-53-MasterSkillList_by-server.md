# C2 F3 53 - MasterSkillList (by server)

## Is sent when

Usually after entering the game with a master character.

## Causes the following actions on the client side

The data is available in the master skill tree.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x53  | Packet header - sub packet type identifier |
| 8 | 4 | IntegerLittleEndian |  | MasterSkillCount |
| 12 | MasterSkillEntry.Length * MasterSkillCount | Array of MasterSkillEntry |  | Skills |

### MasterSkillEntry Structure

An entry in the master skill list.

Length: 12 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | MasterSkillIndex; The index of the master skill on the clients master skill tree for the given character class. |
| 1 | 1 | Byte |  | Level |
| 4 | 4 | Float |  | DisplayValue |
| 8 | 4 | Float |  | DisplayValueOfNextLevel |