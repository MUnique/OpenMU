# C3 F3 03 - CharacterInformationExtended (server gửi)

## Được gửi khi nào

Sau khi nhân vật được người chơi lựa chọn và vào game.

## Hành động phía client

Các nhân vật bước vào thế giới trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 96 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x03 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | X |
| 5 | 1 | Byte |  | Y |
| 6 | 2 | ShortLittleEndian |  | Id bản đồ |
| 8 | 8 | LongBigEndian |  | Kinh nghiệm hiện tại |
| 16 | 8 | LongBigEndian |  | ExperienceForNextLevel |
| 24 | 2 | ShortLittleEndian |  | LevelUpPoints |
| 26 | 2 | ShortLittleEndian |  | Sức mạnh |
| 28 | 2 | ShortLittleEndian |  | Nhanh nhẹn |
| 30 | 2 | ShortLittleEndian |  | Sức sống |
| 32 | 2 | ShortLittleEndian |  | Năng lượng |
| 34 | 2 | ShortLittleEndian |  | Khả năng lãnh đạo |
| 36 | 4 | IntegerLittleEndian |  | Sức khỏe hiện tại |
| 40 | 4 | IntegerLittleEndian |  | Sức khỏe tối đa |
| 44 | 4 | IntegerLittleEndian |  | Mana hiện tại |
| 48 | 4 | IntegerLittleEndian |  | Mana tối đa |
| 52 | 4 | IntegerLittleEndian |  | CurrentShield |
| 56 | 4 | IntegerLittleEndian |  | Lá chắn tối đa |
| 60 | 4 | IntegerLittleEndian |  | Khả năng hiện tại |
| 64 | 4 | IntegerLittleEndian |  | Khả năng tối đa |
| 68 | 4 | IntegerLittleEndian |  | Tiền bạc |
| 72 | 1 | CharacterHeroState |  | anh hùngnhà nước |
| 73 | 1 | CharacterStatus |  | Trạng thái |
| 74 | 2 | ShortLittleEndian |  | Điểm trái cây đã qua sử dụng |
| 76 | 2 | ShortLittleEndian |  | Điểm trái cây tối đa |
| 78 | 2 | ShortLittleEndian |  | Đã sử dụngĐiểm trái cây tiêu cực |
| 80 | 2 | ShortLittleEndian |  | MaxTiêu cựcFruitPoints |
| 82 | 2 | ShortLittleEndian |  | Tốc độ tấn công |
| 84 | 2 | ShortLittleEndian |  | Phép thuậtTốc độ |
| 86 | 2 | ShortLittleEndian |  | Tốc độ tấn công tối đa |
| 88 | 1 | Byte |  | Tiện ích mở rộng khoảng không quảng cáo |

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