# C3 73 - GameGuardAuthRequest (by server)

## Is sent when

The server sends an authentication query of the anti cheat system (nProtect GameGuard).

## Causes the following actions on the client side

The client passes the data to GameGuard; the answer is sent as C3 73 00.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x73  | Packet header - packet type identifier |
| 4 |  | Binary |  | Data |