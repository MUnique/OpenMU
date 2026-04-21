# C2 31 - StoreItemList (server gửi)

## Được gửi khi nào

Người chơi mở một npc thương gia hoặc kho tiền. Nó được gửi sau khi hộp thoại được mở bằng một tin nhắn khác.

## Hành động phía client

Máy khách hiển thị các mục trong hộp thoại đã mở.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x31 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | ItemWindow |  | Type |
| 5 | 1 | Byte |  | số lượng vật phẩm |
| 6 | StoredItem.Length * ItemCount | Array of StoredItem |  | Mặt hàng |

### Cấu trúc StoredItem
Cấu trúc cho một mục được lưu trữ, ví dụ: trong kho hoặc kho tiền.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Khe vật phẩm |
| 1 |  | Binary |  | Dữ liệu mục |

### Enum ItemWindow
Xác định loại cửa sổ npc sẽ được hiển thị trên máy khách.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Một cửa sổ bình thường. |
| 3 | ChaosMachine | Một cửa sổ máy hỗn loạn. |
| 5 | ResurrectionFailed | Hộp thoại lưu trữ hồi sinh không thành công (của Dark Horse hoặc Dark Raven). |