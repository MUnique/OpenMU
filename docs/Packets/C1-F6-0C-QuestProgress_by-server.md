# C1 F6 0C - QuestProgress (by server)

## Is sent when

First, after the game client requested to initialize a quest and the quest is already active. Second, after the game client requested the next quest step.

## Causes the following actions on the client side

The client shows the quest progress accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   251   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0C  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |
| 8 | 1 | Byte |  | ConditionCount |
| 9 | 1 | Byte |  | RewardCount |
| 11 | QuestCondition.Length * ConditionCount | Array of QuestCondition |  | Conditions |
| 141 | QuestReward.Length * RewardCount | Array of QuestReward |  | Rewards |

### QuestCondition Structure

Defines a condition which must be fulfilled to complete the quest.

Length: 26 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | ConditionType |  | Type |
| 4 | 2 | ShortLittleEndian |  | RequirementId; Depending on the condition type, this field contains the identifier of the required thing, e.g. Monster Number, Item Id, Level. |
| 6 | 4 | IntegerLittleEndian |  | RequiredCount |
| 10 | 4 | IntegerLittleEndian |  | CurrentCount |
| 14 | 12 | Binary |  | RequiredItemData; If the condition type is 'Item', this field contains the item data, excluding the item id. The item id can be found in the RequirementId field. |

### ConditionType Enum

Defines the type of the condition.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | No condition is defined. |
| 1 | MonsterKills | The condition is to kill a specified amount of specified monsters. |
| 2 | Skill | The condition is to learn a specified skill. |
| 4 | Item | The condition is to find a specified item and to have it in the inventory. |
| 8 | Level | The condition is to reach the specified character level. |
| 16 | ClientAction | The condition is a client action. For example, this may be the completion of a tutorial. |
| 32 | RequestBuff | The condition is to request a buff from an NPC. |
| 64 | EventMapPlayerKills | The condition is to kill a specific amount of players in an event. |
| 65 | EventMapMonsterKills | The condition is to kill a specific amount of monsters in an event. |
| 66 | BloodCastleGate | The condition is to destroy the blood castle gate. |
| 256 | WinBloodCastle | The condition is to win the blood castle event. |
| 257 | WinChaosCastle | The condition is to win the chaos castle. |
| 258 | WinDevilSquare | The condition is to win the devil square event. |
| 259 | WinIllusionTemple | The condition is to win the illusion temple event. |
| 260 | DevilSquarePoints | The condition is to reach a specific amount of points in the devil square event. |
| 261 | Money | The condition is to give a specific amount of zen. |
| 262 | PvpPoints | The condition is to reach a specific amount of PVP points. |
| 263 | NpcTalk | The condition is to talk to a specific NPC. |

### QuestReward Structure

Defines a reward which is given when the quest is completed.

Length: 22 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | RewardType |  | Type |
| 4 | 2 | ShortLittleEndian |  | RewardId; Depending on the condition type, this field contains the identifier of the required thing, e.g. Monster Number, Item Id, Level. |
| 6 | 4 | IntegerLittleEndian |  | RewardCount |
| 10 | 12 | Binary |  | RewardedItemData; If the reward type is 'Item', this field contains its item data. |

### RewardType Enum

Defines the reward which is given when the quest is completed.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | No reward is defined. |
| 1 | Experience | The reward is added experience to the character. |
| 2 | Money | The reward is added money to the inventory. |
| 4 | Item | The reward is an item which is added to the inventory. |
| 16 | GensContribution | The reward is added gens contribution points. |
| 32 | Random | The reward is random?. |