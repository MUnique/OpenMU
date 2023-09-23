# C1 F6 0A - QuestSelectRequest (by client)

## Is sent when

The client opened an quest NPC dialog and selected an available quests.

## Causes the following actions on the server side

If the quest is already active, it responds with the QuestProgress. If the quest is inactive, the server decides if the character can start the quest and responds with a QuestStepInfo with the StartingNumber. A character can run up to 3 concurrent quests at a time.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |
| 8 | 1 | Byte |  | SelectedTextIndex; A 1-based index of the selected index in the dialog. It's 0 when no text has been selected. It's not clear yet, when we need that. |