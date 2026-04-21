# C1 F3 06 - CharacterStatIncreaseResponse (server gửi)

## Được gửi khi nào

Sau khi server xử lý packet yêu cầu tăng điểm chỉ số nhân vật.

## Hành động phía client

Nếu thành công, client cộng thêm 1 điểm vào stat đã yêu cầu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x06 | Packet header - sub packet type identifier |
| 4 << 4 | 1 bit | Boolean |  | Success |
| 4 | 4 bit | CharacterStatAttribute |  | Attribute |
| 6 | 2 | ShortLittleEndian |  | UpdatedDependentMaximumStat |
| 8 | 2 | ShortLittleEndian |  | UpdatedMaximumShield |
| 10 | 2 | ShortLittleEndian |  | UpdatedMaximumAbility |

### Enum CharacterStatAttribute

Định nghĩa loại stat của nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Strength | Sức mạnh. |
| 1 | Agility | Nhanh nhẹn. |
| 2 | Vitality | Thể lực. |
| 3 | Energy | Năng lượng. |
| 4 | Leadership | Chỉ huy. |
