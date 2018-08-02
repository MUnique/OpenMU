# C1 F6 0F - Quest Cancel Request

## Is sent when
The clients requests to cancel a quest.


## Causes the following actions on the server side
The server checks if the quest is currently in progress. In this case, the quest state is reset
and a [response](<C1F60F - Quest cancel response (by server).md>) is sent back to the client.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x08   | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x0F   | Packet header - packet type identifier |
| 2 | short |       | Quest Number (big endian) |
| 2 | short |       | Quest Group (big endian) |

## Example
```
C1 08 F6 0F     // Header
33 00           // Quest Number
12 00           // Quest Group
```
