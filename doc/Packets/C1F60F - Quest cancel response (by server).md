# C1 F6 0F - Quest Cancel Response

## Is sent when
The server acknowledges that the [requested](<C1F60F - Quest cancel request (by client).md>) quest was cancelled.


## Causes the following actions on the client side
The client resets the state of the quest on its side and can [request](<C1F630 - Available Quests Request (by client).md>) a
new list of available quests again (probably containing the same quest again).


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte | 0x08    | Packet header - length of the packet |
| 1 | byte | 0xF6    | Packet header - packet type identifier |
| 1 | byte | 0x0F    | Packet header - packet type identifier |
| 2 | short | 0x0000 | New Current Quest Number, logically always 0 |
| 2 | short |        | Quest Group (big endian) |

## Example
```
C1 08 F6 0F     // Header
00 00           // Quest Number
12 00           // Quest Group
```
