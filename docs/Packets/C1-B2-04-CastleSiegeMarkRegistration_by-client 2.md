# C1 B2 04 - CastleSiegeMarkRegistration (by client)

## Is sent when

The player opened a castle siege npc and adds a guild mark to his guilds registration.

## Causes the following actions on the server side

The server returns a response, which includes the number of submitted guild marks.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ItemIndex |