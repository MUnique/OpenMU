# C1 A1 - LegacyQuestStateDialog (by server)

## Is sent when

When the player clicks on the quest npc.

## Causes the following actions on the client side

The game client shows the next steps in the quest dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA1  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | QuestIndex |
| 4 | 1 | Byte |  | State; This is the complete byte with the state of four quests within the same byte. |