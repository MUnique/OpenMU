# C2 E9 - AllianceList (server gửi)

## Được gửi khi nào

Một người chơi đã yêu cầu hộp thoại danh sách liên minh.

## Hành động phía client

Khách hàng hiển thị danh sách các bang hội trong liên minh.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0xE9 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte |  | Bang hội |
| 5 | 1 | Boolean |  | Thành công |
| 6 | 1 | Byte |  | __Số lượng đối thủ |
| 7 | 1 | Byte |  | __Đếm Liên minh |
| 8 | AllianceGuildEntry.Length * GuildCount | Array of AllianceGuildEntry |  | Bang hội |

### Cấu trúc AllianceGuildEntry
Chứa dữ liệu của một mục nhập bang hội liên minh.

Độ dài: 41 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Số lượng thành viên |
| 1 | 32 | Binary |  | biểu tượng |
| 33 | 8 | String |  | Tên bang hội |