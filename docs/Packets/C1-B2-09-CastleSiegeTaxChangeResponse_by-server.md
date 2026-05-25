# C1 B2 09 - CastleSiegeTaxChangeResponse (by server)

## Is sent when

After the guild master changed the tax rate.

## Causes the following actions on the client side

The client shows the result of the tax rate change.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 1 | Byte |  | TaxType |
| 6 | 4 | IntegerBigEndian |  | TaxRate |