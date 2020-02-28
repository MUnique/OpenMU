# C3 24 - ItemMoveRequest (by client)

## Is sent when

A player requests to move an item within or between his available item storage, such as inventory, vault, trade or chaos machine.

## Causes the following actions on the server side



## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x24  | Packet header - packet type identifier |
| 3 | 1 | ItemStorageKind |  | FromStorage |
| 4 | 1 | Byte |  | FromSlot |
| 5 | 12 | Binary |  | ItemData |
| 17 | 1 | ItemStorageKind |  | ToStorage |
| 18 | 1 | Byte |  | ToSlot |

### ItemStorageKind Enum

Defines from or to which item storage an item is moved.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Inventory | The inventory storage |
| 1 | Trade | The trade storage |
| 2 | Vault | The vault storage |
| 3 | ChaosMachine | The chaos machine storage |
| 4 | PlayerShop | The player shop storage |