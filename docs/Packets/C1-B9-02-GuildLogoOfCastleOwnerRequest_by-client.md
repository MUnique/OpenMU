# C1 B9 02 - GuildLogoOfCastleOwnerRequest (by client)

## Is sent when

The client requests the guild logo of the current castle owner guild.

## Causes the following actions on the server side

The server returns the guild logo.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB9  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |