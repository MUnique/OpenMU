# C1 F6 0A - QuestStartRequest (by client)

## Is sent when

The client opened an quest NPC dialog and decided to start an available quests.

## Causes the following actions on the server side

The server decides if the character can start the quest. A character can run up to 3 concurrent quests at a time.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |
| 8 | 1 | Byte |  | UnknownField; A value between 1 and 3, probably depending on how many quests are already running. Should not be trusted or considered. |