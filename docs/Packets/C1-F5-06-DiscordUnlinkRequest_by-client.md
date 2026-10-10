# C1 F5 06 - DiscordUnlinkRequest (by client)

## Is sent when

The player wants to remove the link of the account to a Discord user.

## Causes the following actions on the server side

The server removes the link and sends an updated DiscordIntegrationInfo message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |