# C1 BC - LahapJewelMixRequest (client gửi)

## Được gửi khi nào

Khi người chơi mở hộp thoại Lahap npc và muốn kết hợp hoặc giải tán các ngăn xếp ngọc.

## Hành động phía server

Nếu thành công, kho đồ sẽ được cập nhật và ứng dụng khách trò chơi sẽ nhận được phản hồi tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 7 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xBC | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | MixType |  | Hoạt động |
| 4 | 1 | ItemType |  | Mục |
| 5 | 1 | StackSize |  | TrộnStackKích thước |
| 6 | 1 | Byte |  | UnmixingNguồnKhe cắm |

### Enum MixType
Mô tả loại hoạt động được yêu cầu.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Mix | Người chơi muốn trộn các viên ngọc đơn lẻ thành một bó. |
| 1 | Unmix | Người chơi muốn chia một bó thành những viên ngọc duy nhất. |

### Enum StackSize
Mô tả kích thước của một ngăn xếp hỗn hợp.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Ten | Một chồng gồm 10 món. |
| 1 | Twenty | Một chồng gồm 20 mặt hàng. |
| 2 | Thirty | Một chồng gồm 30 mặt hàng. |

### Enum ItemType
Mô tả loại mặt hàng được trộn lẫn hoặc không trộn lẫn.

| Value | Name | Description |
|-------|------|-------------|
| 0 | JewelOfBless | Viên ngọc ban phước. |
| 1 | JewelOfSoul | Viên ngọc của tâm hồn. |
| 2 | JewelOfLife | Viên ngọc của cuộc sống. |
| 3 | JewelOfCreation | Viên ngọc sáng tạo. |
| 4 | JewelOfGuardian | Viên ngọc của người giám hộ. |
| 5 | Gemstone | Đá quý. |
| 6 | JewelOfHarmony | Viên ngọc hài hòa. |
| 7 | JewelOfChaos | Viên ngọc hỗn loạn. |
| 8 | LowerRefineStone | Hạ đá tinh chế. |
| 9 | HigherRefineStone | Đá tinh chế cao hơn. |