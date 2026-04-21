# C3 3F 07 - PlayerShopCloseOther (client gửi)

## Được gửi khi nào

Một người chơi đóng hộp thoại của cửa hàng người chơi khác.

## Hành động phía server

Máy chủ xử lý việc đó bằng cách hủy đăng ký người chơi khỏi những thay đổi của cửa hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 16 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x07 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id người chơi |
| 6 | 10 | String |  | Tên người chơi |