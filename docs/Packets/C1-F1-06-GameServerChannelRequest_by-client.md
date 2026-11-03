# C1 F1 06 - GameServerChannelRequest (by client)

## Is sent when

Sent when the player confirms the game server channel message box.

## Causes the following actions on the server side

The server answers with GameServerChannelResult (F1 06).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Value |