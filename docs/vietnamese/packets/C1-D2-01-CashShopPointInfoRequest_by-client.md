# C1 D2 01 - CashShopPointInfoRequest (client gửi)

## Được gửi khi nào

Khách hàng cần thông tin về số lượng điểm mua hàng bằng tiền mặt (WCoinC, WCoinP, GoblinPoints) có sẵn cho người chơi.

## Hành động phía server

Máy chủ trả về thông tin điểm cửa hàng tiền mặt.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |