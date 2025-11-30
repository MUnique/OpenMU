# C1 F3 40 - ItemPostRequest (by client)

## Is sent when

The player posts an item link into the chat by pressing <kbd>Ctrl</kbd> + <kbd>Right Click</kbd> on an item.

## Causes the following actions on the server side

The server stores the referenced item and broadcasts a chat message containing the generated post id.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x40  | Packet header - sub packet type identifier |
| 4 | 1 | ItemStorageKind |  | StorageType; Determines from which storage the item should be taken. |
| 5 | 1 | Byte |  | ItemSlot; Slot index of the posted item within the chosen storage. |

