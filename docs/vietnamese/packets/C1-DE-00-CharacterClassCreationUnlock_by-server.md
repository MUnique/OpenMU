# C1 DE 00 - CharacterClassCreationUnlock (server gửi)

## Được gửi khi nào

Nó được gửi ngay sau Danh sách ký tự, trong màn hình chọn ký tự, nếu tài khoản có bất kỳ lớp ký tự nào được mở khóa.

## Hành động phía client

Máy khách mở khóa các lớp ký tự được chỉ định để có thể tạo chúng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xDE | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | CharacterCreationUnlockFlags |  | Mở khóaCờ |

### Enum CharacterCreationUnlockFlags
Cờ mở khóa các lớp ký tự được chỉ định để tạo ký tự mới.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | Không có lớp mở khóa. |
| 1 | Summoner | Mở khóa lớp người triệu hồi. |
| 2 | DarkLord | Mở khóa lớp chúa tể bóng tối. |
| 4 | MagicGladiator | Mở khóa lớp đấu sĩ ma thuật. |
| 8 | RageFighter | Mở khóa lớp chiến binh cuồng nộ. |