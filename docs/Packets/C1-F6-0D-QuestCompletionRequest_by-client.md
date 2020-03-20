# C1 F6 0D - QuestCompletionRequest (by client)

## Is sent when

The game client requests to complete an active quest.

## Causes the following actions on the server side

The server checks the conditions to complete the quest. If this fails, nothing happens. If all conditions are met, the reward is given to the player and the quest state is set accordingly, so that the player can select to start the next quest. Additionally, the quest completion response message (F60D) is sent to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0D  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |