# C1 F6 1A - QuestStateList (server gửi)

## Được gửi khi nào

Sau khi khách hàng trò chơi yêu cầu danh sách tất cả các nhiệm vụ hiện đang được thực hiện hoặc được chấp nhận.

## Hành động phía client

Không rõ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x1A | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | nhiệm vụ đếm |
| 5 | QuestIdentification.Length * QuestCount | Array of QuestIdentification |  | Nhiệm vụ |

### Cấu trúc QuestIdentification
Xác định thông tin xác định một nhiệm vụ.

Độ dài: 4 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | Con số |
| 2 | 2 | ShortLittleEndian |  | Nhóm |