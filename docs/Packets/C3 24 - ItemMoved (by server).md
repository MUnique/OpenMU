# C3 24 - ItemMoved (by server)

## Is sent when

An item in the inventory or vault of the player has been moved.

## Causes the following actions on the client side

The client updates the position of item in the user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x24  | Packet header - packet type identifier |
| 3 | 1 | ItemStorageKind |  | TargetStorageType |
| 4 | 1 | Byte |  | TargetSlot |
| 5 |  | Binary |  | ItemData |

### ItemStorageKind Enum

Defines from or to which item storage an item is moved.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Inventory | The inventory storage |
| 1 | Trade | The trade storage |
| 2 | Vault | The vault storage |
| 3 | ChaosMachine | The chaos machine storage |
| 4 | PlayerShop | The player shop storage |