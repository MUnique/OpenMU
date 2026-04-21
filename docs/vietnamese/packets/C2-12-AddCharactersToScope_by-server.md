# C2 12 - AddCharactersToScope (server gửi)

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
| 4 | 1 | Byte |  | Số ký tự |
| 5 | CharacterData.Length * CharacterCount | Array of CharacterData |  | nhân vật |

### Cấu trúc CharacterData
Chứa dữ liệu của NPC.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | Vị trí hiện tạiX |
| 3 | 1 | Byte |  | Vị trí hiện tạiY |
| 4 | 18 | Binary |  | Vẻ bề ngoài |
| 22 | 10 | String |  | Name |
| 32 | 1 | Byte |  | Vị trí mục tiêuX |
| 33 | 1 | Byte |  | Vị trí mục tiêuY |
| 34 | 4 bit | Byte |  | Xoay |
| 34 << 0 | 4 bit | CharacterHeroState |  | anh hùngnhà nước |
| 35 | 1 | Byte |  | Đếm hiệu ứng; Xác định số lượng hiệu ứng sẽ được gửi sau trường này. |
| 36 | EffectId.Length * EffectCount | Array of EffectId |  | Các hiệu ứng |

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

### Cấu trúc EffectId
Chứa id của hiệu ứng ma thuật.

Độ dài: 1 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Id |