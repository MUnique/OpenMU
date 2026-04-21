# C1 25 - AppearanceChanged (server gửi)

## Được gửi khi nào

Ngoại hình của một người chơi đã thay đổi, tất cả những người chơi xung quanh đều được thông báo về điều đó.

## Hành động phía client

Sự xuất hiện của người chơi được cập nhật.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x25 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Id người chơi đã thay đổi |
| 5 |  | Binary |  | Dữ liệu mục |