# C1 F6 0D - QuestCompletionResponse (by server)

## Is sent when

The server acknowledges the completion of a quest, or refuses it because the rewards don't fit.

## Causes the following actions on the client side

When the quest was completed, the client shows the success and possibly requests for the next available quests. When it was refused, the client shows a message and disables the button to complete the quest.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0D  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |
| 8 | 1 | QuestCompletionResult |  | Result |

### QuestCompletionResult Enum

Defines the result of a quest completion request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NotCompleted | The quest wasn't completed. |
| 1 | Completed | The quest was completed. |
| 2 | ZenLimitReached | The quest wasn't completed, because the money of the rewards would exceed the money limit of the inventory. |
| 3 | InventoryFull | The quest wasn't completed, because the item rewards don't fit into the inventory. |