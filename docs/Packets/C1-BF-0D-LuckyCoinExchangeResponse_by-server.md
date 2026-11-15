# C1 BF 0D - LuckyCoinExchangeResponse (by server)

## Is sent when

After the player requested to exchange lucky coins for an item at the NPC Delgado.

## Causes the following actions on the client side

The client unlocks the exchange buttons and shows a message about the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0D  | Packet header - sub packet type identifier |
| 4 | 1 | LuckyCoinExchangeResult |  | Result |

### LuckyCoinExchangeResult Enum

Defines the result of the lucky coin exchange request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NotEnoughItems | The player doesn't have enough lucky coins in the inventory. |
| 1 | Exchanged | The lucky coins were exchanged for an item. |
| 2 | InventoryFull | The inventory has not enough space for the item. The client asks for a free space of 2 x 4. |