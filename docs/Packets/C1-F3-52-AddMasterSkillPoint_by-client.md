# C1 F3 52 - AddMasterSkillPoint (by client)

## Is sent when

The player wants to add or increase the level of a specific master skill of the master skill tree.

## Causes the following actions on the server side

Adds or increases the master skill level of the specified skill, if the character is allowed to do that. A response is sent back to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x52  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | SkillId |