# C1 18 - AnimationRequest (client gửi)

## Được gửi khi nào

Người chơi thực hiện bất kỳ loại hoạt ảnh nào.

## Hành động phía server

Số hoạt ảnh và vòng quay được chuyển tiếp đến tất cả người chơi xung quanh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x18 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Xoay |
| 4 | 1 | Byte |  | Hoạt HìnhSố |