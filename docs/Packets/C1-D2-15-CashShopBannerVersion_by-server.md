# C1 D2 15 - CashShopBannerVersion (by server)

## Is sent when

The player entered the game.

## Causes the following actions on the client side

The client remembers the version of the cash shop banner. When the dialog is opened, the client loads the banner of this version from 'Data\InGameShopBanner\[SaleZone].[Year].[YearId]'.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x15  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | SaleZone |
| 6 | 2 | ShortLittleEndian |  | Year |
| 8 | 2 | ShortLittleEndian |  | YearId |