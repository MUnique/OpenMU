# C1 F6 30 - AvailableQuestsRequest (by client)

## Is sent when

The client opened an quest NPC dialog and requests a list of available quests.

## Causes the following actions on the server side

The list of available quests of this NPC is sent back (F60A).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x30  | Packet header - sub packet type identifier |