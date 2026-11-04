# C2 F6 1B - QuestStateExtended (by server)

## Is sent when

After the game client requested it, when the player opened the quest menu and clicked on a quest.

## Causes the following actions on the client side

The client shows the quest progress accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |   272   | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x1B  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | ConditionCount |
| 6 | 1 | Byte |  | RewardCount |
| 7 | 1 | Byte |  | RandomRewardCount |
| 8 | 2 | ShortLittleEndian |  | QuestNumber |
| 10 | 2 | ShortLittleEndian |  | QuestGroup |
| 12 | QuestConditionExtended.Length * ConditionCount | Array of QuestConditionExtended |  | Conditions |
| 152 | QuestRewardExtended.Length * RewardCount | Array of QuestRewardExtended |  | Rewards |

### QuestConditionExtended Structure

Defines a condition which must be fulfilled to complete the quest.

Length: 28 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | ConditionType |  | Type |
| 2 | 2 | ShortLittleEndian |  | RequirementId; Depending on the condition type, this field contains the identifier of the required thing, e.g. Monster Number, Item Id, Level. |
| 4 | 4 | IntegerLittleEndian |  | RequiredCount |
| 8 | 4 | IntegerLittleEndian |  | CurrentCount |
| 12 | 15 | Binary |  | RequiredItemData; If the condition type is 'Item', this field contains the item data, excluding the item id. The item id can be found in the RequirementId field. |

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

### QuestRewardExtended Structure

Defines a reward which is given when the quest is completed.

Length: 24 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | RewardType |  | Type |
| 2 | 2 | ShortLittleEndian |  | RewardId; Depending on the condition type, this field contains the identifier of the required thing, e.g. Monster Number, Item Id, Level. |
| 4 | 4 | IntegerLittleEndian |  | RewardCount |
| 8 | 15 | Binary |  | RewardedItemData; If the reward type is 'Item', this field contains its item data. |

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