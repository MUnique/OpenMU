# C1 F3 30 - ApplyKeyConfiguration (server gửi)

## Được gửi khi nào

Khi bước vào thế giới game với một nhân vật.

## Hành động phía client

Máy khách khôi phục cấu hình này trong giao diện người dùng của nó.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x30 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 |  | Binary |  | Cấu hình; Dữ liệu nhị phân của cấu hình khóa |