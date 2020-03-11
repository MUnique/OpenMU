# C1 C7 - LetterReadRequest (by client)

## Is sent when

A player requests to read a specific letter of his letter list.

## Causes the following actions on the server side

The server sends the requested letter content back to the game client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC7  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | LetterIndex |