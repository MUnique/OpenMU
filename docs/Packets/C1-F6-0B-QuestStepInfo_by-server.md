# C1 F6 0B - QuestStepInfo (by server)

## Is sent when

After the game client clicked on a quest in the quest list, proceeded with a quest or refused to start a quest.

## Causes the following actions on the client side

The client shows the corresponding description about the current quest step.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   11   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0B  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestStepNumber; A number specifying the description: A) when selecting a quest in the quest list, it's the "StartingNumber"; B) when a quest has been started it's the quest number; C) when the starting number has been sent previously and the player refused to start the quest, it sends a "RefuseNumber". |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |