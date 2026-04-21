# C1 B7 01 - FireCatapultRequest (client gửi)

## Được gửi khi nào

Người chơi muốn bắn máy phóng trong sự kiện bao vây lâu đài.

## Hành động phía server

Máy chủ kích hoạt máy phóng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB7 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id máy phóng |
| 6 | 1 | Byte |  | Chỉ số khu vực mục tiêu |