# C1 F6 0A - AvailableQuests (by server)

## Is sent when

After the game client requested the list of available quests through an NPC dialog.

## Causes the following actions on the client side

The client shows the available quests for the currently interacting NPC.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNpcNumber |
| 6 | 2 | ShortLittleEndian |  | QuestCount |
| 8 | QuestIdentification.Length * QuestCount | Array of QuestIdentification |  | Quests |

### QuestIdentification Structure

Defines the information which identifies a quest.

Length: 4 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | Number |
| 2 | 2 | ShortLittleEndian |  | Group |