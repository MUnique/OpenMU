# C1 BF 04 - IllusionTempleResult (server gửi)

## Được gửi khi nào

Sự kiện ngôi đền ảo ảnh đã kết thúc.

## Hành động phía client

Khách hàng hiển thị kết quả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x04 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Đội1Điểm |
| 5 | 1 | Byte |  | Đội2Points |
| 6 | 1 | Byte |  | Số lượng người chơi |
| 10 | PlayerResult.Length * PlayerCount | Array of PlayerResult |  | Người chơi |

### Cấu trúc PlayerResult
Chứa kết quả của người chơi trong sự kiện.

Độ dài: 17 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 |  | String |  | Name |
| 10 | 1 | Byte |  | Bản đồSố |
| 11 | 1 | Byte |  | Đội |
| 12 | 1 | Byte |  | Lớp học |
| 13 | 4 | IntegerLittleEndian |  | Đã thêm kinh nghiệm |