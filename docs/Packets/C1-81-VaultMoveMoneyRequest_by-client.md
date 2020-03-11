# C1 81 - VaultMoveMoneyRequest (by client)

## Is sent when

The player wants to move money from or to the vault storage.

## Causes the following actions on the server side

The money is moved, if possible.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x81  | Packet header - packet type identifier |
| 3 | 1 | VaultMoneyMoveDirection |  | Direction |
| 4 | 4 | IntegerLittleEndian |  | Amount |

### VaultMoneyMoveDirection Enum

Defines the moving direction of money between inventory and vault.

| Value | Name | Description |
|-------|------|-------------|
| 0 | InventoryToVault | The money is moved from the inventory to the vault. |
| 1 | VaultToInventory | The money is moved from the vault to the inventory. |