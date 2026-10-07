# C1 D2 0C - CashShopScriptVersion (by server)

## Is sent when

The player entered the game.

## Causes the following actions on the client side

The client remembers the version of the cash shop script (product catalog) and unlocks the cash shop. When the dialog is opened, the client loads the script of this version from 'Data\InGameShopScript\[SaleZone].[Year].[YearId]'.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0C  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | SaleZone |
| 6 | 2 | ShortLittleEndian |  | Year |
| 8 | 2 | ShortLittleEndian |  | YearId |