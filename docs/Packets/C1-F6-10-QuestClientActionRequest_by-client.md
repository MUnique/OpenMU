# C1 F6 10 - QuestClientActionRequest (by client)

## Is sent when

The game client requests to complete a client action, e.g. completing a tutorial.

## Causes the following actions on the server side

The server checks if the specified quest is currently in progress. If the quest got a Condition (condition type 0x10) for this flag, the condition is flagged as fulfilled.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x10  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | QuestNumber |
| 6 | 2 | ShortLittleEndian |  | QuestGroup |