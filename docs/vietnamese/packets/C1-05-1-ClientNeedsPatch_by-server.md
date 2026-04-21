# C1 05 1 - ClientNeedsPatch (server gửi)

## Được gửi khi nào

Gói này được máy chủ gửi sau khi máy khách (trình khởi chạy) yêu cầu kiểm tra phiên bản vá lỗi và nó yêu cầu cập nhật.

## Hành động phía client

Trình khởi chạy sẽ tải xuống các bản vá cần thiết và sau đó kích hoạt nút bắt đầu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 138 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x1 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Phiên bản vá |
| 6 |  | String |  | Địa chỉ Patch; Địa chỉ bản vá, thường là máy chủ ftp. Địa chỉ thường được "mã hóa" bằng khóa XOR 3 byte (FC CF AB). |