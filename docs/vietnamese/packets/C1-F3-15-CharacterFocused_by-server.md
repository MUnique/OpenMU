# C1 F3 15 - CharacterFocused (server gửi)

## Được gửi khi nào

Sau khi máy khách tập trung ký tự thành công vào phía máy chủ.

## Hành động phía client

Khách hàng làm nổi bật nhân vật tập trung.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 15 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x15 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 10 | String |  | Tên nhân vật |