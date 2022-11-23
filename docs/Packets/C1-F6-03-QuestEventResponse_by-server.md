# C1 F6 03 - QuestEventResponse (by server)

## Is sent when

After the game client requested the list of event quests after entering the game. It seems to be sent only if the character is not a member of a Gen.

## Causes the following actions on the client side

Unknown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | QuestIdentification.Length *  | Array of QuestIdentification |  | Quests |

### QuestIdentification Structure

Defines the information which identifies a quest.

Length: 4 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | Number |
| 2 | 2 | ShortLittleEndian |  | Group |