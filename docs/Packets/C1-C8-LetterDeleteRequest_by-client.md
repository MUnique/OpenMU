# C1 C8 - LetterDeleteRequest (by client)

## Is sent when

A player requests to delete a letter.

## Causes the following actions on the server side

The letter is getting deleted.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC8  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | LetterIndex |