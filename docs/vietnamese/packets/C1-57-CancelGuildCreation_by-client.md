# C1 57 - CancelGuildCreation (client gửi)

## Được gửi khi nào

Người chơi mở hộp thoại tạo bang hội và quyết định không tạo bang hội.

## Hành động phía server

Nó hoặc hủy bỏ việc tạo bang hội.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x57 | Tiêu đề gói - mã định danh loại gói |