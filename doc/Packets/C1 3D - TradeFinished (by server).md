# C1 3D - TradeFinished (by server)

## Is sent when

A trade was finished.

## Causes the following actions on the client side

The trade dialog is closed. Depending on the result, a message is shown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3D  | Packet header - packet type identifier |
| 4 | 1 | TradeResult |  | Result |

### TradeResult Enum

Defines the result of a finished trade.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Cancelled | The trade was cancelled. |
| 1 | Success | The trade was successful. |
| 2 | FailedByFullInventory | The trade failed because of a full inventory. |
| 3 | TimedOut | The trade failed because the request timed out. |
| 4 | FailedByItemsNotAllowedToTrade | The trade failed because one or more items were not allowed to trade. |