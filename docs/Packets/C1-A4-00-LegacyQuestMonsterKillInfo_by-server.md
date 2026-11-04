# C1 A4 00 - LegacyQuestMonsterKillInfo (by server)

## Is sent when

As response when a player opens the quest npc with a running quest which requires monster kills.

## Causes the following actions on the client side

The game client shows the current state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   48   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA4  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | Byte | 1 | Result |
| 5 | 1 | Byte |  | QuestIndex |
| 8 | MonsterKillInfo.Length *  | Array of MonsterKillInfo |  | Kills |

### MonsterKillInfo Structure

A pair of Monster number and the current kill count.

Length: 8 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | MonsterNumber |
| 4 | 4 | IntegerLittleEndian |  | KillCount |