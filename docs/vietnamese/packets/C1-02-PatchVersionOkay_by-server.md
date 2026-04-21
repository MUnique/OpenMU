# C1 02 - PatchVersionOkay (server gửi)

## Được gửi khi nào

Gói này được máy chủ gửi sau khi máy khách (trình khởi chạy) yêu cầu kiểm tra phiên bản vá lỗi và nó đủ cao.

## Hành động phía client

Trình khởi chạy sẽ kích hoạt nút bắt đầu của nó.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói |