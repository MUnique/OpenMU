# C1 03 - ChecksumRequest (by server)

## Is sent when

The server requests a checksum of the client executable.

## Causes the following actions on the client side

The client calculates the checksum for the given key and answers with a ChecksumResponse (C3 03).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x03  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | Key |