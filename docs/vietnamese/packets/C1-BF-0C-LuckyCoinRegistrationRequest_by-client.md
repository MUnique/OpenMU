# C1 BF 0C - LuckyCoinRegistrationRequest (client gửi)

## Được gửi khi nào

Người chơi mở hộp thoại đồng xu may mắn và yêu cầu đăng ký một đồng xu may mắn có trong kho của mình.

## Hành động phía server

Máy chủ trả về kết quả đăng ký làm tăng số lượng xu và giảm độ bền của xu đi một.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0C | Tiêu đề gói - mã định danh loại gói phụ |