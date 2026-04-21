# C1 F3 11 - SkillListUpdate (server gửi)

## Được gửi khi nào

Thông thường, khi người chơi vào game với một nhân vật. Khi các kỹ năng được thêm hoặc xóa, thông báo này cũng được gửi đi nhưng với số lượng sai lệch.

## Hành động phía client

Danh sách kỹ năng được khởi tạo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x11 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Đếm; Cách sử dụng hỗn hợp: Đếm danh sách kỹ năng (khi danh sách). 0xFE khi thêm kỹ năng, 0xFF khi xóa Kỹ năng. |
| 6 | SkillEntry.Length * Count | Array of SkillEntry |  | Kỹ năng |

### Cấu trúc SkillEntry
Cấu trúc cho mục nhập kỹ năng của danh sách kỹ năng.

Độ dài: 4 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Chỉ số kỹ năng |
| 1 | 2 | ShortLittleEndian |  | Số kỹ năng |
| 3 | 1 | Byte |  | Cấp độ kỹ năng |