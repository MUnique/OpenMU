# C2 1F - AddSummonedMonstersToScope (server gửi)

## Được gửi khi nào

Một hoặc nhiều quái vật được triệu hồi đã lọt vào phạm vi quan sát của người chơi.

## Hành động phía client

Khách hàng thêm quái vật vào bản đồ được hiển thị.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x1F | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte |  | Đếm quái vật |
| 5 | SummonedMonsterData.Length * MonsterCount | Array of SummonedMonsterData |  | Triệu HồiQuái Vật |

### Cấu trúc SummonedMonsterData
Chứa dữ liệu của NPC.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 2 | 2 | ShortBigEndian |  | LoạiSố |
| 4 | 1 | Byte |  | Vị trí hiện tạiX |
| 5 | 1 | Byte |  | Vị trí hiện tạiY |
| 6 | 1 | Byte |  | Vị trí mục tiêuX |
| 7 | 1 | Byte |  | Vị trí mục tiêuY |
| 8 | 4 bit | Byte |  | Xoay |
| 9 | 10 | String |  | Tên nhân vật chủ sở hữu |
| 19 | 1 | Byte |  | Đếm hiệu ứng; Xác định số lượng hiệu ứng sẽ được gửi sau trường này. Điều này hiện không được hỗ trợ. |
| 20 | EffectId.Length * EffectCount | Array of EffectId |  | Các hiệu ứng |

### Cấu trúc EffectId
Chứa id của hiệu ứng ma thuật.

Độ dài: 1 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Id |