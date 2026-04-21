# C1 2D - EffectItemConsumption (server gửi)

## Được gửi khi nào

Người chơi yêu cầu tiêu thụ một vật phẩm mang lại hiệu ứng ma thuật.

## Hành động phía client

Client cập nhật giao diện người dùng, nó hiển thị thời gian còn lại tại biểu tượng hiệu ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 17 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x2D | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | EffectOrigin |  | Nguồn gốc |
| 6 | 1 | EffectType |  | Type |
| 8 | 1 | EffectAction |  | Hoạt động |
| 12 | 4 | IntegerLittleEndian |  | Số giây còn lại |
| 16 | 1 | Byte |  | Phép ThuậtHiệu ỨngSố |

### Enum EffectOrigin
Xác định nguồn gốc của hiệu ứng.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Không được xác định. |
| 1 | HalloweenAndCherryBlossomEvent | Tùy chọn vật phẩm Sự kiện Halloween và Hoa anh đào. |
| 2 | CashShopItem | Tùy chọn các mặt hàng trong cửa hàng tiền mặt, như Seals. |

### Enum EffectAction
Xác định tùy chọn hiệu ứng.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Add | Hiệu ứng được thêm vào. |
| 1 | Remove | Hiệu ứng được loại bỏ. |
| 2 | Replace | Hiệu ứng bị loại bỏ vì nó được thay thế. |

### Enum EffectType
Xác định loại hiệu ứng được áp dụng.

| Value | Name | Description |
|-------|------|-------------|
| 1 | AttackSpeed | Tăng tốc độ tấn công. |
| 2 | Damage | Sát thương tăng lên. |
| 3 | Defense | Phòng thủ tăng lên. |
| 4 | MaximumHealth | Tăng sức khỏe tối đa. |
| 5 | MaximumMana | Tăng mana tối đa. |
| 6 | ExperienceRate | Tăng tỷ lệ kinh nghiệm. |
| 7 | DropRate | Tăng tỷ lệ rớt. |
| 8 | Sustenance | Hiệu ứng duy trì, có nghĩa là không có kinh nghiệm nào đạt được trong hiệu ứng này. |
| 9 | Strength | Tăng chỉ số sức mạnh. |
| 10 | Agility | Tăng chỉ số nhanh nhẹn. |
| 11 | Vitality | Tăng chỉ số sức sống. |
| 12 | Energy | Tăng chỉ số năng lượng. |
| 13 | Leadership | Tăng chỉ số lãnh đạo. |
| 14 | PhysicalDamage | Tăng sát thương vật lý. |
| 15 | WizardryDamage | Tăng sát thương phép thuật. |
| 16 | Mobility | Tính di động tăng lên. |