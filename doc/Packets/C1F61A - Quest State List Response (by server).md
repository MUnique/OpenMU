# C1 F6 1A - Quest State List Response

## Is sent when
After the client [requested](<C1F61A - Quest State List Request (by client).md>) the list of all quests which are in progress or accepted.


## Causes the following actions on the client side
Unknown.

## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | [Length] | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x1A   | Packet header - packet type identifier |
| 1 | byte |       | Count of quests in progress |
| 4 * n | Quest |   | One block per Quest |

### Quest Block
|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 2 | short |    | Quest Number |
| 2 | short |    | Quest Group |


## Example
```
C1 0D F6 1A // Header
02 // Count of quests in progress
// First Quest Block:
0B 00 // Quest Number
10 00 // Quest Group
// Second Quest Block:
0A 00 // Quest Number
11 00 // Quest Group
```