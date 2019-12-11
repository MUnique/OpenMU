# C1 F6 0C - Quest Progress

## Is sent when
 * After a client [requested](<C1F60A - Quest Initialization Request (by client).md>) to initialize a quest and the quest is already active.
 * After a client requested the next quest step.


## Causes the following actions on the client side
The client shows the quest progress accordingly.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0xF1   | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x0C   | Packet header - packet type identifier |
| 2 | short |       | Quest Number (big endian) |
| 2 | short |       | Quest Group (big endian) |
| 1 | byte |        | Number of Conditions in the following array |
| 1 | byte |        | Number of Rewards in the following array |
| 1 | byte |        | padding |
| 5 | Condition[] | | Quest completion conditions, 5 times  |
| 5 | Reward[]|     | Quest completion reward, 5 times |

### Condition

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 4 | int |   | Condition Type, see below |
| 2 | short | | Requirement Id (item id, skill id, level) |
| 4 | int |   | Required (item count, kills, true/false for skill, level)  | 
| 4 | int |   | Current (item count, kills, true/false for skill, level)  |
| 10 | byte[] | | Required Item, if condition type is Item |

#### Condition Type

| Value | Description |
|-------|-------------|
| 0x00  | None        |
| 0x01  | Monster kills |
| 0x02  | Skill       |
| 0x04  | Item        |
| 0x08  | Level       |
| 0x10  | Client Action |
| 0x20  | Request Buff  |



### Reward
|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 4 | int |   | Reward Type, see below |
| 2 | short | | Reward Id |
| 4 | int |   | Reward Count (e.g. Number of Items, Experience, Money )  |
| 12 | byte[] | | Rewarded Item |

#### Reward Type

| Value | Description |
|-------|-------------|
|   0   | None        |
|   1   | Experience  |
|   2   | Money       |
|   4   | Item        |
|  16   | Gens Contribution |


## Example
```
C1 F1 F6 0C // Header
34 00       // Quest Number
12 00       // Quest Group
01          // One Condition
01          // One Reward
00          // padding

// First Condition:
01 00 00 00                     // 1 -> Type: Monster kills
0A 00                           // Monsters of type "Dark Knight"
32 00 00 00                     // 50 kills required
02 00 00 00                     // 2 kills currently
00 00 00 00 00 00 00 00 00 00   // Empty item data, probably excluding item number and group. This would probably be contained in the Requirement Id, if applicable.

00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 // Empty Condition #2
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 // Empty Condition #3
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 // Empty Condition #4
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 // Empty Condition #5

// First Reward:
01 00 00 00                         // 1 -> Type: Experience
00 00                               // Experience doesn't have an id
D0 01 01 00                         // 66,000 Experience
00 00 00 00 00 00 00 00 00 00 00 00 // Empty item data

00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 // Empty Reward #2
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 // Empty Reward #3
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 // Empty Reward #4
00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 // Empty Reward #5
```