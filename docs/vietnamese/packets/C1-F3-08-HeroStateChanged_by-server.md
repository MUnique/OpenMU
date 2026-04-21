# C1 F3 08 - HeroStateChanged (server gửi)

## Được gửi khi nào

Sau khi trạng thái anh hùng của một nhân vật được quan sát thay đổi.

## Hành động phía client

Màu sắc của tên nhân vật được thay đổi tương ứng và hiển thị thông báo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x08 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortBigEndian |  | Id người chơi |
| 6 | 1 | CharacterHeroState |  | Bang mới |

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