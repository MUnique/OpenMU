# C1 57 - CancelGuildCreation (by client)

## Is sent when

The player has the dialog of the guild creation dialog opened and decided against creating a guild.

## Causes the following actions on the server side

It either cancels the guild creation.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x57  | Packet header - packet type identifier |