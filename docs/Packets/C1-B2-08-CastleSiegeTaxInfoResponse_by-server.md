# C1 B2 08 - CastleSiegeTaxInfoResponse (by server)

## Is sent when

After the guild master opened the castle npc to manage the castle taxes.

## Causes the following actions on the client side

The client shows the current tax configuration and treasury amount.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   19   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x08  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | TaxRateChaosMachine |
| 5 | 1 | Byte |  | TaxRateNormal |
| 6 | 4 | IntegerBigEndian |  | TaxRateLandOfTrials |
| 10 | 1 | Boolean |  | IsPublicHuntingAllowed |
| 11 | 8 | LongBigEndian |  | Treasury |