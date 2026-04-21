# C1 48 - ShowEffect (server gửi)

## Được gửi khi nào

Sau khi người chơi đạt được hoặc mất thứ gì đó.

## Hành động phía client

Một hiệu ứng được hiển thị cho người chơi bị ảnh hưởng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x48 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Id người chơi |
| 5 | 1 | EffectType |  | Tác dụng |

### Enum EffectType
Xác định hiệu ứng được hiển thị cho người chơi.

| Value | Name | Description |
|-------|------|-------------|
| 3 | ShieldPotion | Người chơi nhận được lá chắn bằng cách uống một lọ thuốc. |
| 16 | LevelUp | Hiệu ứng tăng cấp được hiển thị cho người chơi. |
| 17 | ShieldLost | Khiên của người chơi cạn kiệt. |