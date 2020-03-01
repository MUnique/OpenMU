# C1 A0 - LegacyQuestStateRequest (by client)

## Is sent when

After the player entered the game world with a character.

## Causes the following actions on the server side

The quest state is sent back as response.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xA0  | Packet header - packet type identifier |