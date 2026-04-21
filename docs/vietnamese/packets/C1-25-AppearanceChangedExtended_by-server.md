# C1 25 - AppearanceChangedExtended (server gửi)

## Được gửi khi nào

Ngoại hình của một người chơi đã thay đổi, tất cả những người chơi xung quanh đều được thông báo về điều đó.

## Hành động phía client

Sự xuất hiện của người chơi được cập nhật.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 14 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x25 | Tiêu đề gói - mã định danh loại gói |
| 4 | 2 | ShortLittleEndian |  | Id người chơi đã thay đổi |
| 6 | 1 | Byte |  | Khe vật phẩm |
| 7 | 1 | Byte |  | Nhóm mục |
| 8 | 2 | ShortLittleEndian |  | Số hạng mục |
| 10 | 1 | Byte |  | Cấp độ vật phẩm |
| 11 | 1 | Byte |  | Tuyệt vờiCờ |
| 12 | 1 | Byte |  | Cổ xưaNgười phân biệt đối xử |
| 13 | 1 | Boolean |  | LàCổBộHoàn Thành |