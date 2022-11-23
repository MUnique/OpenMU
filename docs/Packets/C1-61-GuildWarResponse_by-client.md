# C1 61 - GuildWarResponse (by client)

## Is sent when

A guild master requested a guild war against another guild.

## Causes the following actions on the server side

If the guild master confirms, the war is declared.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x61  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Accepted |