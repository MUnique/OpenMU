# C1 A3 - LegacyQuestReward (by server)

## Is sent when

As response to the completed quest of a player in scope.

## Causes the following actions on the client side

The game client shows the reward accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA3  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | PlayerId |
| 5 | 1 | QuestRewardType |  | Reward |
| 6 | 1 | Byte |  | Count |

### QuestRewardType Enum

Defines the reward type in the quest reward message.

| Value | Name | Description |
|-------|------|-------------|
| 200 | LevelUpPoints | The character receives additional points. |
| 201 | CharacterEvolutionFirstToSecond | The character class changes from the first to the second class. |
| 202 | LevelUpPointsPerLevelIncrease | The character receives additional points per level. |
| 203 | ComboSkill | The character receives the ability to perform skill combinations. |
| 204 | CharacterEvolutionSecondToThird | The character class changes from the second to the third class. |