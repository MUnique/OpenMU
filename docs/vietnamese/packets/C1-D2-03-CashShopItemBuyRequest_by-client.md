# C1 D2 03 - CashShopItemBuyRequest (client gửi)

## Được gửi khi nào

Khi người chơi muốn mua item trong cash shop.

## Hành động phía server

Server mua item và thêm item vào kho cash shop của người chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 23 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xD2 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x03 | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | PackageMainIndex |
| 8 | 4 | IntegerLittleEndian |  | Category |
| 12 | 4 | IntegerLittleEndian |  | ProductMainIndex |
| 16 | 2 | ShortLittleEndian |  | ItemIndex |
| 18 | 4 | IntegerLittleEndian |  | CoinIndex |
| 22 | 1 | Byte |  | MileageFlag |
