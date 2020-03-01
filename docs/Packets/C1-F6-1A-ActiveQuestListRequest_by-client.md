# C1 F6 1A - ActiveQuestListRequest (by client)

## Is sent when

The clients requests the states of all quests, usually after entering the game.

## Causes the following actions on the server side

The list of active quests is sent back (F61A) without changing any state. This list just contains all running or completed quests for each group.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x1A  | Packet header - sub packet type identifier |