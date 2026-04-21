# C1 BF 0B - LuckyCoinCountRequest (client gửi)

## Được gửi khi nào

Người chơi mở hộp thoại đồng xu may mắn và yêu cầu số lượng xu đã đăng ký hiện tại.

## Hành động phía server

Máy chủ trả về số lượng tiền đã đăng ký.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0B | Tiêu đề gói - mã định danh loại gói phụ |