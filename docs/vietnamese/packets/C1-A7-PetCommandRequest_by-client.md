# C1 A7 - PetCommandRequest (client gửi)

## Được gửi khi nào

Người chơi muốn chỉ huy thú cưng được trang bị của mình (quạ).

## Hành động phía server



## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA7 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | PetType |  | Loại thú cưng |
| 4 | 1 | PetCommandMode |  | Chế độ lệnh |
| 5 | 2 | ShortBigEndian |  | Id mục tiêu |

### Enum PetType
Mô tả loại vật nuôi.

| Value | Name | Description |
|-------|------|-------------|
| 0 | DarkRaven | Con quạ đen tối. |
| 1 | DarkHorse | Con ngựa đen thú cưng. |

### Enum PetCommandMode
Mô tả chế độ lệnh của thú cưng.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Thú cưng ở chế độ bình thường và không tấn công. |
| 1 | AttackRandom | Thú cưng tấn công các mục tiêu ngẫu nhiên. |
| 2 | AttackWithOwner | Thú cưng tấn công các mục tiêu giống như chủ nhân. |
| 3 | AttackTarget | Thú cưng tấn công một mục tiêu cụ thể cho đến khi nó chết. |