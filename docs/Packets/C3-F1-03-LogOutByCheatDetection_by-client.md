# C3 F1 03 - LogOutByCheatDetection (by client)

## Is sent when

When the client wants to leave the game in various ways.

## Causes the following actions on the server side

Depending on the LogOutType, the game server does several checks and sends a response back to the client. If the request was successful, the game client either closes the game, goes back to server or character selection.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 1 | Byte | 4 | Type |
| 5 | 1 | Byte |  | Param |