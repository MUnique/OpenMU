# C2 F3 42 - ItemPostInfo (by server)

## Is sent when

A client requested the tooltip data of a posted item link.

## Causes the following actions on the client side

The client shows the posted item tooltip using the provided serialized item data.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x42  | Packet header - sub packet type identifier |
| 5 |  | Binary |  | ItemData; Serialized item data which should be displayed in the tooltip. |

