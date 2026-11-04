# C1 F5 00 - ChatCommandListRequest (by client)

## Is sent when

A client which supports a user interface for chat commands requests the list of commands which are available to the player. It's usually sent after the character entered the game world.

## Causes the following actions on the server side

The server sends an AvailableChatCommand message for each available chat command.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |