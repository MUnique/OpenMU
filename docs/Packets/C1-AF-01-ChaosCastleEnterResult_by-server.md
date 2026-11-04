# C1 AF 01 - ChaosCastleEnterResult (by server)

## Is sent when

The player requested to enter the chaos castle mini game by using the 'Armor of Guardsman' item.

## Causes the following actions on the client side

In case it failed, it shows the corresponding error message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | EnterResult |  | Result |

### EnterResult Enum

Defines the result of the enter request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The event has been entered. |
| 1 | Failed | Entering the event failed, e.g. by missing event ticket or level range. |
| 2 | NotOpen | Entering the event failed, because it's not opened. |
| 5 | Full | Entering the event failed, because it's full. |
| 7 | NotEnoughMoney | Entering the event failed, because the player doesn't have enough money for the entrance fee. |
| 8 | PlayerKillerCantEnter | Entering the event failed, because the player has a pk state. |