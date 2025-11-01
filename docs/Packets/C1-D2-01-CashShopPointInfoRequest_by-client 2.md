# C1 D2 01 - CashShopPointInfoRequest (by client)

## Is sent when

The client needs information about how many cash shop points (WCoinC, WCoinP, GoblinPoints) are available to the player.

## Causes the following actions on the server side

The server returns the cash shop points information.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |