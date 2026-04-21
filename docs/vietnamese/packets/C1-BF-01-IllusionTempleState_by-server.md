# C1 BF 01 - IllusionTempleState (server gửi)

## Được gửi khi nào

Người chơi đang tham gia sự kiện ngôi đền ảo ảnh và máy chủ sẽ gửi bản cập nhật theo chu kỳ.

## Hành động phía client

Máy khách hiển thị trạng thái trong giao diện người dùng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Số giây còn lại |
| 4 | 2 | ShortLittleEndian |  | Chỉ số người chơi |
| 6 | 1 | Byte |  | Vị tríX |
| 7 | 1 | Byte |  | Vị tríY |
| 8 | 1 | Byte |  | Đội1Điểm |
| 9 | 1 | Byte |  | Đội2Points |
| 10 | 1 | Byte |  | Đội của tôi |
| 11 | 1 | Byte |  | Số lượng bữa tiệc |
| 12 | IllusionTemplePartyEntry.Length * | Array of IllusionTemplePartyEntry |  | ĐảngThành viên |

### Cấu trúc IllusionTemplePartyEntry
Chứa thông tin về một thành viên trong nhóm ảo ảnh.

Độ dài: 5 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | Id người chơi |
| 2 | 2 | ShortLittleEndian |  | Bản đồSố |
| 3 | 1 | Byte |  | Vị tríX |
| 4 | 1 | Byte |  | Vị tríY |