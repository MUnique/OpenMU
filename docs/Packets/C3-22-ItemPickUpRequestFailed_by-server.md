# C3 22 - ItemPickUpRequestFailed (by server)

## Is sent when

The player requested to pick up an item from to ground to add it to his inventory, but it failed.

## Causes the following actions on the client side

Depending on the reason, the game client shows a message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x22  | Packet header - packet type identifier |
| 3 | 1 | ItemPickUpFailReason |  | FailReason |

### ItemPickUpFailReason Enum

Defines the possible fail reasons

| Value | Name | Description |
|-------|------|-------------|
| 253 | ItemStacked | The picked up item was combined into an existing item of the players inventory. A separate durability update will be sent to the client. |
| 254 | __MaximumInventoryMoneyReached | The maximum inventory money has been reached, so the money wasn't picked up. Should not be used, because it's used in the InventoryMoneyUpdate message. |
| 255 | General | The general, non-specific reason. It just failed. |