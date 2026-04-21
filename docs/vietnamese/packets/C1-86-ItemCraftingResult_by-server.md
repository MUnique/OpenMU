# C1 86 - ItemCraftingResult (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu thực hiện chế tạo vật phẩm, ví dụ: tại cỗ máy hỗn loạn.

## Hành động phía client

Ứng dụng khách trò chơi cập nhật giao diện người dùng để hiển thị mục kết quả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x86 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | CraftingResult |  | Kết quả |
| 4 |  | Binary |  | Dữ liệu mục |

### Enum CraftingResult
Xác định kết quả chế tạo.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failed | Việc chế tạo thất bại. |
| 1 | Success | Việc chế tạo đã thành công. |
| 2 | NotEnoughMoney | Việc chế tạo không được thực hiện vì thiếu tiền. |
| 3 | TooManyItems | Việc chế tạo không được thực hiện vì có quá nhiều vật phẩm. |
| 4 | CharacterLevelTooLow | Việc chế tạo không được thực hiện vì cấp độ nhân vật quá thấp. |
| 6 | LackingMixItems | Việc chế tạo không được thực hiện vì thiếu vật phẩm. |
| 7 | IncorrectMixItems | Việc chế tạo không được thực hiện do các vật phẩm không chính xác. |
| 8 | InvalidItemLevel | Việc chế tạo không được thực hiện do cấp độ vật phẩm không hợp lệ. |
| 9 | CharacterClassTooLow | Việc chế tạo không được thực hiện vì lớp nhân vật quá thấp. |
| 10 | IncorrectBloodCastleItems | Việc tạo vé lâu đài máu không được thực hiện vì các vật phẩm trong Lâu đài máu không chính xác. |
| 11 | NotEnoughMoneyForBloodCastle | Việc chế tạo không được thực hiện vì người chơi không có đủ tiền để chế tạo vé lâu đài máu. |