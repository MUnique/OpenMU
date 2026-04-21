# C3 F3 03 - CharacterInformation097 (server gửi)

## Được gửi khi nào

Sau khi nhân vật được người chơi lựa chọn và vào game.

## Hành động phía client

Các nhân vật bước vào thế giới trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 52 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x03 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | X |
| 5 | 1 | Byte |  | Y |
| 6 | 1 | Byte |  | Id bản đồ |
| 7 | 1 | Byte |  | Phương hướng |
| 8 | 4 | IntegerLittleEndian |  | Kinh nghiệm hiện tại |
| 12 | 4 | IntegerLittleEndian |  | ExperienceForNextLevel |
| 16 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 18 | 2 | ShortLittleEndian |  | Sức mạnh |
| 20 | 2 | ShortLittleEndian |  | Nhanh nhẹn |
| 22 | 2 | ShortLittleEndian |  | Sức sống |
| 24 | 2 | ShortLittleEndian |  | Năng lượng |
| 26 | 2 | ShortLittleEndian |  | Sức khỏe hiện tại |
| 28 | 2 | ShortLittleEndian |  | Sức khỏe tối đa |
| 30 | 2 | ShortLittleEndian |  | Mana hiện tại |
| 32 | 2 | ShortLittleEndian |  | Mana tối đa |
| 34 | 2 | ShortLittleEndian |  | Khả năng hiện tại |
| 36 | 2 | ShortLittleEndian |  | Khả năng tối đa |
| 40 | 4 | IntegerLittleEndian |  | Tiền bạc |
| 44 | 1 | CharacterHeroState |  | anh hùngnhà nước |
| 45 | 1 | CharacterStatus |  | Trạng thái |
| 46 | 2 | ShortLittleEndian |  | Điểm trái cây đã qua sử dụng |
| 48 | 2 | ShortLittleEndian |  | Điểm trái cây tối đa |
| 50 | 2 | ShortLittleEndian |  | Khả năng lãnh đạo |

### Enum CharacterHeroState
Xác định trạng thái anh hùng của một nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | New | Nhân vật mới và có trạng thái cao nhất. |
| 1 | Hero | Nhân vật là một anh hùng. |
| 2 | LightHero | Nhân vật là anh hùng nhưng nhà nước gần như không còn nữa. |
| 3 | Normal | Nhân vật ở trạng thái trung lập. |
| 4 | PlayerKillWarning | Nhân vật này đã giết một nhân vật khác và có cảnh báo giết. |
| 5 | PlayerKiller1stStage | Nhân vật đã giết hai nhân vật và có một số hạn chế. |
| 6 | PlayerKiller2ndStage | Nhân vật đã giết nhiều hơn hai nhân vật và có những hạn chế cứng nhắc. |

### Enum CharacterStatus
Trạng thái của một nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Trạng thái của nhân vật là bình thường. |
| 1 | Banned | Nhân vật bị cấm khỏi trò chơi. |
| 32 | GameMaster | Nhân vật là một bậc thầy trò chơi. |