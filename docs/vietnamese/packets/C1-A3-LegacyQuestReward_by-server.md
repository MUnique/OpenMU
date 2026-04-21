# C1 A3 - LegacyQuestReward (server gửi)

## Được gửi khi nào

Như một phản hồi cho nhiệm vụ đã hoàn thành của người chơi trong phạm vi.

## Hành động phía client

Ứng dụng khách trò chơi sẽ hiển thị phần thưởng tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Id người chơi |
| 5 | 1 | QuestRewardType |  | Phần thưởng |
| 6 | 1 | Byte |  | Đếm |

### Enum QuestRewardType
Xác định loại phần thưởng trong thông báo phần thưởng nhiệm vụ.

| Value | Name | Description |
|-------|------|-------------|
| 200 | LevelUpPoints | Nhân vật nhận được điểm bổ sung. |
| 201 | CharacterEvolutionFirstToSecond | Lớp nhân vật thay đổi từ lớp thứ nhất sang lớp thứ hai. |
| 202 | LevelUpPointsPerLevelIncrease | Nhân vật nhận được điểm bổ sung cho mỗi cấp độ. |
| 203 | ComboSkill | Nhân vật nhận được khả năng thực hiện kết hợp kỹ năng. |
| 204 | CharacterEvolutionSecondToThird | Lớp nhân vật thay đổi từ lớp thứ hai sang lớp thứ ba. |