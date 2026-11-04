# C1 D2 13 - CashShopEventItemListCount (by server)

## Is sent when

After the player requested the event item list.

## Causes the following actions on the client side

The client prepares the event item list.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x13  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | Count |