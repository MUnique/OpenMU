# C1 F6 0A - Quest List Response

## Is sent when
After the client [requested](<C1F630 - Available Quests Request (by client).md>) the list of available quests
through an NPC dialog.


## Causes the following actions on the client side
The client shows the available quests for the currently interacting NPC.

## Additional Informations
Since this message contains just a quest "number" and "group", the client and server must
have the same informations (descriptions etc.) about these quests already.

There can only be one quest available of each group. If a quest of a group was already started, the corresponding quest number is returned here, too.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | [Length] | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x0A   | Packet header - packet type identifier |
| 2 | short |       | Number of the opened quest npc |
| 2 | short |       | Count of available quests |
| 4 * n | Quest |   | One block per available Quest |

### Quest Block
|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 2 | short |    | Quest Number |
| 2 | short |    | Quest Group |


## Example
```
C1 0C F6 0A // Header
01 01 // NPC Id
01 00 // Count of available Quests
// First Quest Block:
33 00 // Quest Number
12 00 // Quest Group
```