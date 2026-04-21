# C4 F3 10 - CharacterInventory (server gửi)

## Được gửi khi nào

Người chơi tham gia trò chơi hoặc hoàn thành giao dịch.

## Hành động phía client

Giao diện người dùng của kho được khởi tạo với tất cả các mặt hàng của nó.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC4 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0xF3 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte | 0x10 | Tiêu đề gói - mã định danh loại gói phụ |
| 5 | 1 | Byte |  | số lượng vật phẩm |
| 6 | StoredItem.Length * ItemCount | Array of StoredItem |  | Mặt hàng |

### Cấu trúc StoredItem
Cấu trúc cho một mục được lưu trữ, ví dụ: trong kho hoặc kho tiền.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Khe vật phẩm |
| 1 |  | Binary |  | Dữ liệu mục |