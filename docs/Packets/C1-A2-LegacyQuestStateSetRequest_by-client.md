# C1 A2 - LegacyQuestStateSetRequest (by client)

## Is sent when

The player wants to change the state of a quest, e.g. to start or to finish a quest.

## Causes the following actions on the server side

Depending on the requested new state, a response is sent back.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA2  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | QuestNumber |
| 4 | 1 | LegacyQuestState |  | NewState |

### LegacyQuestState Enum

The quest state for the legacy quest system.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The state is undefined. This value is used when there is no quest defined for a specific index |
| 1 | Active | The quest is active and in progress. |
| 2 | Complete | The quest was completed. |
| 3 | Inactive | The quest is inactive, that means it isn't active and wasn't completed yet. |