# C1 F6 0B - Quest Proceed Request

## Is sent when
After the server [started](<C1F60B - Quest Start (by server).md>) the quest, the
client may choose to proceed.


## Causes the following actions on the server side
The quest state is set accordingly on the server. The next response seems to depend
on the quest configuration. Depending on the action of the next quest state,
the server will send either a [quest progress](<C1F60C - Quest Progress (by server).md>) or again a [quest start](<C1F60B - Quest Start (by server).md>)


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x09   | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x0B   | Packet header - packet type identifier |
| 2 | short |       | Quest Number (big endian) |
| 2 | short |       | Quest Group (big endian) |
| 1 | byte |        | Next quest state |

## Example
```
C1 09 F6 0B     // Header
33 00           // Quest Number
12 00           // Quest Group
01              // Next quest state
```
