# C3 16 - ExperienceGainedExtended (server gửi)

## Được gửi khi nào

Một người chơi đã có được kinh nghiệm.

## Hành động phía client

Kinh nghiệm được thêm vào quầy và thanh kinh nghiệm.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 16 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x16 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | AddResult |  | Type |
| 4 | 4 | IntegerLittleEndian |  | Đã thêm kinh nghiệm |
| 8 | 4 | IntegerLittleEndian |  | Sát thương của lần truy cập cuối cùng |
| 12 | 2 | ShortLittleEndian |  | Id đối tượng bị giết |
| 14 | 2 | ShortLittleEndian |  | Id đối tượng sát thủ |

### Enum AddResult
Xác định kết quả và loại trải nghiệm được thêm vào.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Không xác định, không có kinh nghiệm được thêm vào. |
| 1 | Normal | Trải nghiệm bình thường được thêm vào. |
| 2 | Master | Trải nghiệm chính được thêm vào. |
| 16 | MaxLevelReached | Đã đạt đến cấp độ tối đa, không có kinh nghiệm được thêm vào. |
| 32 | MaxMasterLevelReached | Đã đạt đến cấp độ chủ tối đa, không có kinh nghiệm chủ nào được thêm vào. |
| 33 | MonsterLevelTooLowForMasterExperience | Cấp độ quái vật quá thấp để có được kinh nghiệm chủ nhân, không có kinh nghiệm chủ nhân nào được thêm vào. |