# C1 F6 1B - Quest State Request

## Is sent when
The clients requests the state data of a quest.


## Causes the following actions on the server side
The server answers with a [response](<C1F61B - Quest State Response (by server).md>) without changing any state,
if the quest is currently in progress.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x08   | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x1B   | Packet header - packet type identifier |
| 2 | short |       | Quest Number (big endian) |
| 2 | short |       | Quest Group (big endian) |

## Example
```
C1 08 F6 1B     // Header
33 00           // Quest Number
12 00           // Quest Group
```
