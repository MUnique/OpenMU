# C1 F6 0D - QuestCompletionResponse (by server)

## Is sent when

The server acknowledges the completion of a quest.

## Causes the following actions on the client side

The client shows the success and possibly requests for the next available quests.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0D  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |
| 8 | 1 | Boolean |  | IsQuestCompleted |