# C1 00 01 - Hello (server gửi)

## Được gửi khi nào

Gói này được gửi bởi máy chủ sau khi máy khách kết nối với máy chủ.

## Hành động phía client

Một khách hàng trò chơi sẽ yêu cầu danh sách máy chủ. Trình khởi chạy sẽ yêu cầu trạng thái bản vá.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |