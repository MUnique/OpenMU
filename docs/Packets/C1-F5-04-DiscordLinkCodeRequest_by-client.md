# C1 F5 04 - DiscordLinkCodeRequest (by client)

## Is sent when

The player wants to link the account to a Discord user, e.g. with a button of a Discord dialog.

## Causes the following actions on the server side

The server creates a one-time code and sends it with a DiscordLinkCode message. The player enters the code in Discord to complete the link.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |