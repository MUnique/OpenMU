# C1 A2 - LegacySetQuestStateResponse (by server)

## Is sent when

As response to the set state request (C1A2).

## Causes the following actions on the client side

The game client shows the new quest state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA2  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | QuestIndex |
| 4 | 1 | Byte |  | Result; This value is 0 if successful. Otherwise, 0xFF or even other magic values. |
| 5 | 1 | Byte |  | NewState; This is the complete byte with the state of four quests within the same byte. |