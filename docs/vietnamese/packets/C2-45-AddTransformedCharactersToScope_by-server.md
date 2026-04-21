# C2 45 - AddTransformedCharactersToScope (server gửi)

## Được gửi khi nào

Người chơi đeo một chiếc nhẫn biến hình quái vật.

## Hành động phía client

Nhân vật xuất hiện dưới dạng quái vật, được xác định bởi thuộc tính Skin.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x45 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte |  | Số ký tự |
| 5 | CharacterData.Length * CharacterCount | Array of CharacterData |  | nhân vật |

### Cấu trúc CharacterData
Chứa dữ liệu của một ký tự được chuyển đổi.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 1 | Byte |  | Vị trí hiện tạiX |
| 3 | 1 | Byte |  | Vị trí hiện tạiY |
| 4 | 2 | ShortBigEndian |  | Da |
| 6 | 10 | String |  | Name |
| 16 | 1 | Byte |  | Vị trí mục tiêuX |
| 17 | 1 | Byte |  | Vị trí mục tiêuY |
| 18 | 4 bit | Byte |  | Xoay |
| 18 << 0 | 4 bit | CharacterHeroState |  | anh hùngnhà nước |
| 19 | 18 | Binary |  | Vẻ bề ngoài |
| 37 | 1 | Byte |  | Đếm hiệu ứng; Xác định số lượng hiệu ứng sẽ được gửi sau trường này. |
| 38 | EffectId.Length * EffectCount | Array of EffectId |  | Các hiệu ứng |

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