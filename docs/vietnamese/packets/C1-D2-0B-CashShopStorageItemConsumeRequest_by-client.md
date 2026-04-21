# C1 D2 0B - CashShopStorageItemConsumeRequest (client gửi)

## Được gửi khi nào

Khi người chơi muốn lấy hoặc tiêu thụ một item đang nằm trong kho cash shop.

## Hành động phía server

Server áp dụng item hoặc thêm item vào inventory.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 15 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xD2 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x0B | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | BaseItemCode |
| 8 | 4 | IntegerLittleEndian |  | MainItemCode |
| 12 | 2 | ShortLittleEndian |  | ItemIndex |
| 14 | 1 | Byte |  | ProductType |
