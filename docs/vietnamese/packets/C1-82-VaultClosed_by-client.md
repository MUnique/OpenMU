# C1 82 - VaultClosed (client gửi)

## Được gửi khi nào

Người chơi đã đóng hộp thoại vault đã mở.

## Hành động phía server

Trạng thái trên máy chủ được cập nhật.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x82 | Tiêu đề gói - mã định danh loại gói |