# C1 A4 00 - LegacyQuestMonsterKillInfo (server gửi)

## Được gửi khi nào

Như một phản ứng khi người chơi mở npc nhiệm vụ với một nhiệm vụ đang chạy yêu cầu tiêu diệt quái vật.

## Hành động phía client

Ứng dụng trò chơi hiển thị trạng thái hiện tại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 48 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA4 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte | 1 | Kết quả |
| 5 | 1 | Byte |  | Chỉ mục nhiệm vụ |
| 8 | MonsterKillInfo.Length * | Array of MonsterKillInfo |  | Giết chết |

### Cấu trúc MonsterKillInfo
Một cặp số Quái vật và số lần tiêu diệt hiện tại.

Độ dài: 8 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 4 | IntegerLittleEndian |  | Số Quái Vật |
| 4 | 4 | IntegerLittleEndian |  | Đếm giết |