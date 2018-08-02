# C1 F6 1A - Quest State List Request

## Is sent when
The clients requests the states of all quests, usually after entering the game.


## Causes the following actions on the server side
The server answers with a [response](<C1F61A - Quest State List Response (by server).md>) without changing any state.
This list just contains all running or completed quests for each group.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1   | [Packet type](PacketTypes.md) |
| 1 | byte | 0x04   | Packet header - length of the packet |
| 1 | byte | 0xF6   | Packet header - packet type identifier |
| 1 | byte | 0x1A   | Packet header - packet type identifier |

