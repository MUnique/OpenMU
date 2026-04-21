# C1 86 - ChaosMachineMixRequest (client gửi)

## Được gửi khi nào

Người chơi mở hộp thoại của cỗ máy hỗn loạn và quyết định trộn (chế tạo) các vật phẩm mà mình đưa vào hộp thoại của cỗ máy hỗn loạn.

## Hành động phía server

Dựa trên loại hỗn hợp và tỷ lệ thành công tương ứng mà hỗn hợp thành công hay thất bại. Khách hàng nhận được phản hồi tương ứng với các mục được tạo, thay đổi hoặc bị mất.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x86 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | ChaosMachineMixType |  | Loại hỗn hợp; Mã định danh cho máy chủ biết loại kết hợp nào sẽ được thực thi. |
| 4 | 1 | Byte |  | Khe cắm; Chỉ số khe dựa trên 0 của ổ cắm mà tại đó một quả cầu hạt giống sẽ được gắn hoặc gỡ bỏ. Có thể chỉ có sẵn cho các hỗn hợp tương ứng, vì vậy hãy cẩn thận khi truy cập. |

### Enum ChaosMachineMixType
Enum này mô tả các định danh loại hỗn hợp máy hỗn loạn có thể có.

| Value | Name | Description |
|-------|------|-------------|
| 1 | ChaosWeapon | Trộn các vật phẩm thành vũ khí hỗn loạn. |
| 3 | UpgradeItemLevelTo10 | Nâng cấp một vật phẩm lên cấp 10. |
| 4 | UpgradeItemLevelTo11 | Nâng cấp một vật phẩm lên cấp 11. |
| 22 | UpgradeItemLevelTo12 | Nâng cấp một vật phẩm lên cấp 12. |
| 23 | UpgradeItemLevelTo13 | Nâng cấp một vật phẩm lên cấp 13. |
| 49 | UpgradeItemLevelTo14 | Nâng cấp một vật phẩm lên cấp 14. |
| 50 | UpgradeItemLevelTo15 | Nâng cấp một vật phẩm lên cấp 15. |
| 6 | FruitCreation | Trộn các vật phẩm thành một loại trái cây. |
| 41 | GemstoneRefinery | Tinh chế một viên đá quý thành một viên ngọc hài hòa. |
| 15 | PotionOfBless | Tinh chế Jewel Of Bless thành một đống lọ thuốc ban phước. |
| 16 | PotionOfSoul | Biến Jewel Of Soul thành một đống lọ thuốc ban phước. |