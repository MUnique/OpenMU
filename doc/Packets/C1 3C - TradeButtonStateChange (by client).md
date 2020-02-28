# C1 3C - TradeButtonStateChange (by client)

## Is sent when

The player presses the trade button.

## Causes the following actions on the client side

The state change is forwarded to the trade partner. If both players press the trade button at the same time, the server will try to complete the trade by exchanging the items and money.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x3C  | Packet header - packet type identifier |
| 3 | 1 | TradeButtonState |  | NewState |

### TradeButtonState Enum

The state of the trade button.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Unchecked | Trade button is not pressed. It means that the trade is not yet accepted by the trader. |
| 1 | Checked | Trade Button is pressed. It means that the trade is accepted by the trader. |
| 2 | Red | This state is only sent to the client. After some seconds the client is changing back to normal Unchecked. |