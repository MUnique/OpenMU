# C1 81 - VaultMoneyUpdate (by server)

## Is sent when

After the player requested to move money between the vault and inventory.

## Causes the following actions on the client side

The game client updates the money values of vault and inventory.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x81  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success |
| 4 | 4 | IntegerLittleEndian |  | VaultMoney |
| 8 | 4 | IntegerLittleEndian |  | InventoryMoney |