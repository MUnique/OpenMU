# C1 F6 0F - QuestCancelled (by server)

## Is sent when

The server acknowledges the requested cancellation of a quest.

## Causes the following actions on the client side

The client resets the state of the quest and can request a new list of available quests again. This list would then probably contain the cancelled quest again.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0F  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber; The current quest number. In this message, it's always 0, because the group is relevant for the client. |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |