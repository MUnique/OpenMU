# C1 00 - Authenticate (by client)

## Is sent when

This packet is sent by the client after it connected to the server, to authenticate itself.

## Causes the following actions on the server side

The server will check the token. If it's correct, the client gets added to the requested chat room.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x00  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | RoomId |
| 6 | 10 | String |  | Token; A token (integer number), formatted as string. This value is also "encrypted" with the 3-byte XOR key (FC CF AB). |