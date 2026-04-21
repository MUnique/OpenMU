# C1 D2 02 - CashShopOpenState (client gửi)

## Được gửi khi nào

Người chơi mở hoặc đóng hộp thoại cửa hàng tiền mặt.

## Hành động phía server

Trong trường hợp mở, máy chủ sẽ trả về nếu có cửa hàng rút tiền. Nếu người chơi ở trong vùng an toàn thì không.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Boolean |  | Đã đóng |