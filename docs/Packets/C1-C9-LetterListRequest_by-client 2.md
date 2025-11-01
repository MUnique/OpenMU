# C1 C9 - LetterListRequest (by client)

## Is sent when

The game client requests the current list of letters.

## Causes the following actions on the server side

The server sends the list of available letters to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xC9  | Packet header - packet type identifier |