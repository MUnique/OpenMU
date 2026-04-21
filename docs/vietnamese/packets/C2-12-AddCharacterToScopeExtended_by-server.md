# C2 12 - AddCharacterToScopeExtended (server gửi)

## Được gửi khi nào

Một hoặc nhiều nhân vật lọt vào phạm vi quan sát của người chơi.

## Hành động phía client

Khách hàng thêm ký tự vào bản đồ được hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x12 | Tiêu đề gói - mã định danh loại gói |
| 4 | 2 | ShortLittleEndian |  | Id |
| 6 | 1 | Byte |  | Vị trí hiện tạiX |
| 7 | 1 | Byte |  | Vị trí hiện tạiY |
| 8 | 1 | Byte |  | Vị trí mục tiêuX |
| 9 | 1 | Byte |  | Vị trí mục tiêuY |
| 10 | 4 bit | Byte |  | Xoay |
| 10 << 0 | 4 bit | CharacterHeroState |  | anh hùngnhà nước |
| 12 | 2 | ShortLittleEndian |  | Tốc độ tấn công |
| 14 | 2 | ShortLittleEndian |  | Phép thuậtTốc độ |
| 16 | 10 | String |  | Name |
| 26 |  | Binary |  | Ngoại hình và Hiệu ứng |

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