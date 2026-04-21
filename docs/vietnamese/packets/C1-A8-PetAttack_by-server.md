# C1 A8 - PetAttack (server gửi)

## Được gửi khi nào

Sau khi khách hàng gửi PetAttackCommand, thú cưng sẽ tự động tấn công. Đối với mỗi cuộc tấn công, người chơi và tất cả người chơi quan sát sẽ nhận được thông báo này.

## Hành động phía client

Khách hàng cho thấy thú cưng đang tấn công mục tiêu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 9 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA8 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | ClientToServer.PetType | ClientToServer.PetType.DarkRaven | Thú cưng |
| 4 | 1 | PetSkillType |  | Loại kỹ năng |
| 5 | 2 | ShortBigEndian |  | Id chủ sở hữu |
| 7 | 2 | ShortBigEndian |  | Id mục tiêu |

### Enum PetSkillType
Mô tả kiểu tấn công của thú cưng.

| Value | Name | Description |
|-------|------|-------------|
| 0 | SingleTarget | Một đòn tấn công mục tiêu duy nhất, được sử dụng cho những đòn chí mạng và xuất sắc. |
| 1 | Range | Một cuộc tấn công phạm vi cho nhiều mục tiêu, thường có tối đa 3 mục tiêu bổ sung, tất cả đều nhận được thông báo PetAttack của riêng chúng với 'SingleTarget' ngay sau thông báo 'Phạm vi' đầu tiên. |