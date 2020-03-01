# C1 F6 21 - EventQuestStateListRequest (by client)

## Is sent when

The game client requests the list of event quests, usually after entering the game.

## Causes the following actions on the server side

The server may answer with a response which seems to depend if the character is member of a Gen or not. If it's not in a gen, it sends a response (F603).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x21  | Packet header - sub packet type identifier |