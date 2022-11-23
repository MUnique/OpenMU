# C1 F6 0B - QuestProceedRequest (by client)

## Is sent when

After the server started a quest (and sent a F60B message) the game client requests to proceed with the quest.

## Causes the following actions on the server side

The quest state is set accordingly on the server. The next response seems to depend on the quest configuration. Depending on the action of the next quest state, the server will send either a quest progress message (F60C) or again a quest start message (F60B).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0B  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |
| 8 | 1 | QuestProceedAction |  | ProceedAction |

### QuestProceedAction Enum

Describes how to proceed with the specified quest.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Undefined action. |
| 1 | AcceptQuest | The quest is accepted and started. |
| 2 | RefuseQuest | The quest is refused and not started. |