# C2 F5 02 - WeeklyQuestEntry (by server)

## Is sent when

After the client requested the list of available chat commands, one message is sent for each active weekly quest. When the progress of a quest changes, a single message is sent as update of this quest.

## Causes the following actions on the client side

The client shows the weekly quests and their progress in a window.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |   520   | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | Index; The index of this quest within the list, starting at 0. The first message of a list replaces the previously known quests. |
| 6 | 1 | Byte |  | Count; The total number of quests. A list without quests is sent as a single message with a count of 0. |
| 7 | 1 | Boolean |  | IsUpdate; If true, this message updates the already known quest with the same id, instead of being part of a list. |
| 8 | 1 | Boolean |  | IsCompleted; The objective of the quest has been reached. |
| 9 | 1 | Boolean |  | IsRewarded; The rewards of the quest have been handed out. If the quest is completed but not rewarded, the reward is pending, e.g. because the inventory was full. |
| 12 | 4 | IntegerLittleEndian |  | CurrentCount |
| 16 | 4 | IntegerLittleEndian |  | RequiredCount |
| 20 | 4 | IntegerLittleEndian |  | SecondsUntilReset; The seconds until the weekly progress is reset. |
| 24 | 64 | String |  | Id; The identifier of the quest. |
| 88 | 48 | String |  | Name |
| 136 | 256 | String |  | Description |
| 392 | 128 | String |  | Rewards; The rewards of the quest as text, in the language of the player. |