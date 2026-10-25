# C1 F5 02 - DiscordIntegrationInfoRequest (by client)

## Is sent when

A client which supports the Discord integration requests how the server is connected to Discord. It's usually sent after the character entered the game world, and again when the player opens a Discord dialog. By sending it, the client also announces that it understands the ExternalChatMessage.

## Causes the following actions on the server side

The server sends a DiscordIntegrationInfo message, and from then on sends messages which were written in Discord as ExternalChatMessage.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |