# C2 F5 03 - QuestDetails (by server)

## Is sent when

Right after each WeeklyQuestEntry message, with the same quest id. Clients which don't know it can ignore it and just show the WeeklyQuestEntry.

## Causes the following actions on the client side

The client shows the quest under the tab of its category, with a checklist of its objectives (steps).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | Category; The category of the quest: 0 = weekly, 1 = daily, 2 = main story, 3 = class, 4 = zone. |
| 6 | 1 | Byte |  | Period; When the progress starts over: 0 = weekly, 1 = daily, 2 = never (the quest is done once). |
| 7 | 1 | Byte |  | CurrentStep; The index of the first objective which isn't done yet. It's the number of objectives when all are done. |
| 8 | 1 | Byte |  | ObjectiveCount |
| 9 | 1 | Boolean |  | IsSequential; If true, the objectives have to be done in their order. |
| 12 | 4 | IntegerLittleEndian |  | SecondsUntilReset; The seconds until the progress of this quest starts over. 0 for quests which are done once. |
| 16 | 64 | String |  | Id; The identifier of the quest, as in the WeeklyQuestEntry message. |
| 80 | QuestObjective.Length * ObjectiveCount | Array of QuestObjective |  | Objectives; The objectives of the quest, in their order. |

### QuestObjective Structure

An objective (step) of a quest.

Length: 76 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | CurrentCount |
| 4 | 4 | IntegerLittleEndian |  | RequiredCount |
| 8 | 1 | Boolean |  | IsDone |
| 12 | 64 | String |  | Text; The text of the objective, in the language of the player, e.g. 'Kill 50 Skeletons'. |