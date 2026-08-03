# C1 B2 1A - CastleSiegeTaxRateNotification (by server)

## Is sent when

The castle owner changes a server-wide chaos machine or store tax rate.

## Causes the following actions on the client side

The client updates the applicable tax rate.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x1A  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | TaxType |
| 5 | 1 | Byte |  | TaxRate |