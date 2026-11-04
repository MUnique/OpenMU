# C3 22 FE - InventoryMoneyUpdate (by server)

## Is sent when

The players money amount of the inventory has been changed and needs an update.

## Causes the following actions on the client side

The money is updated in the inventory user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x22  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0xFE  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerBigEndian |  | Money |