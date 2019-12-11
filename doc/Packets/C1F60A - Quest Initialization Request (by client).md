# C1 F6 0A - Quest Initialization Request

## Is sent when
The client opened an quest NPC dialog and decided to start an available quests.


## Causes the following actions on the server side
The server decides if the character can start the quest. A character can run up to 3 concurrent quests at a time.


## Structure

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC1    | [Packet type](PacketTypes.md) |
| 1 | byte |         | Packet header - length of the packet |
| 1 | byte | 0xF6    | Packet header - packet type identifier |
| 1 | byte | 0x0A    | Packet header - packet type identifier |
| 2 | short |        | Quest Number (big endian) |
| 2 | short |        | Quest Group (big endian) |
| 1 | byte |         | 1 ~ 3, I guess depending on how many quests are already running. Should not be trusted or considered. |

