# C1 F3 06 - IncreaseCharacterStatPoint (client gửi)

## Được gửi khi nào

Khi người chơi bấm nút cộng chỉ số trong màn hình thông tin nhân vật để cộng
điểm vào một loại stat cụ thể.

## Hành động phía server

Server kiểm tra còn level-up-point hay không. Nếu có, cộng điểm vào stat được
chỉ định và gửi response về client.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xF3 | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x06 | Packet header - sub packet type identifier |
| 4 | 1 | CharacterStatAttribute |  | StatType |

### Enum CharacterStatAttribute

Định nghĩa loại stat của nhân vật.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Strength | Sức mạnh. |
| 1 | Agility | Nhanh nhẹn. |
| 2 | Vitality | Thể lực. |
| 3 | Energy | Năng lượng. |
| 4 | Leadership | Chỉ huy. |
