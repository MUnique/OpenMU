# C3 03 - ChecksumResponse (by client)

## Is sent when

This packet is sent by the client as a response to a request with a challenge value.

## Causes the following actions on the server side

By the original server, this is used to detect a modified client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x03  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Checksum |