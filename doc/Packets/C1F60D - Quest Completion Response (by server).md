# C1 F6 0D - Quest Completion Response

## Is sent when
The server acknowledges the completion of a quest.


## Causes the following actions on the client side
The client shows the success and possibly requests for the next available quests.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x09   | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x0D   | Packet header - packet type identifier |
| 2 | short |       | Quest Number (big endian) of the completed quest |
| 2 | short |       | Quest Group (big endian) of the completed quest |
| 1 | bool | 0x01   | Success flag |

## Example
```
C1 09 F6 0D     // Header
33 00           // Quest Number
12 00           // Quest Group
01              // Success flag
```
