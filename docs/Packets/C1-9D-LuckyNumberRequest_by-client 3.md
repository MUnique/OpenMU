# C1 9D - LuckyNumberRequest (by client)

## Is sent when

The player requests to redeem a coupon code (lucky number) which is 12 alphanumeric digits long.

## Causes the following actions on the server side

A response is sent back to the client with the result. An item could be rewarded to the inventory.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   18   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x9D  | Packet header - packet type identifier |
| 3 | 4 | String |  | Serial1 |
| 8 | 4 | String |  | Serial2 |
| 13 | 4 | String |  | Serial3 |