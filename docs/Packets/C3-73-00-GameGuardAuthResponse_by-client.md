# C3 73 00 - GameGuardAuthResponse (by client)

## Is sent when

The client answers the GameGuardAuthRequest (C3 73).

## Causes the following actions on the server side

The server can verify the GameGuard answer.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   20   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x73  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 16 | Binary |  | Data |