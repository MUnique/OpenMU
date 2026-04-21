# C1 D2 05 - CashShopStorageListRequest (client gửi)

## Được gửi khi nào

Người chơi mở hộp thoại cửa hàng tiền mặt hoặc sử dụng phân trang của kho lưu trữ.

## Hành động phía server

Trong trường hợp mở, máy chủ sẽ trả về nếu có cửa hàng rút tiền. Nếu người chơi ở trong vùng an toàn thì không.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 10 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xD2 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | chỉ mục trang |
| 8 | 1 | Byte |  | Loại hàng tồn kho |