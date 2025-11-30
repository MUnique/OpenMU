# C1 F3 41 - ItemPostInfoRequest (by client)

## Is sent when

The player hovers a posted item link in the chat to view its tooltip.

## Causes the following actions on the server side

The server looks up the posted item by the given id and responds with the serialized item data.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x41  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerBigEndian |  | PostId; Identifier of the posted item link. |

