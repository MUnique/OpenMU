# C1 F6 04 - QuestStartedByItem (by server)

## Is sent when

After the player used an item which starts a quest.

## Causes the following actions on the client side

The client requests the quest (QuestSelectRequest, F6 0A) with the given quest index.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF6  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 8 | 4 | IntegerLittleEndian |  | QuestIndex; Quest number in the lower and group in the upper 16 bits. |