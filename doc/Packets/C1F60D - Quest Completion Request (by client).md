# C1 F6 0D - Quest Completion Request

## Is sent when
The clients requests to complete a quest.


## Causes the following actions on the server side
The server checks the conditions to complete the quest.
If this fails, nothing happens (TODO: message with success flag false needed?).
If all conditions are met, the reward is given to the player and the quest state
is set accordingly, so that the player can select to start the next quest.
Additionally, the [completion message](<C1F60D - Quest Completion Response (by server).md>) is sent to the client.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x08   | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x0D   | Packet header - packet type identifier |
| 2 | short |       | Quest Number (big endian) |
| 2 | short |       | Quest Group (big endian) |

## Example
```
C1 08 F6 0D     // Header
33 00           // Quest Number
12 00           // Quest Group
```
