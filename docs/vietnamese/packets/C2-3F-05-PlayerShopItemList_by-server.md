# C2 3F 05 - PlayerShopItemList (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu mở cửa hàng của người chơi khác.

## Hành động phía client

Hộp thoại cửa hàng người chơi được hiển thị với dữ liệu vật phẩm được cung cấp.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte | 0x05 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | ActionKind |  | Hoạt động |
| 5 | 1 | Boolean | true | Thành công |
| 6 | 2 | ShortBigEndian |  | Id người chơi |
| 8 | 10 | String |  | Tên người chơi |
| 18 | 36 | String |  | Tên cửa hàng |
| 54 | 1 | Byte |  | số lượng vật phẩm |
| 55 | PlayerShopItem.Length * ItemCount | Array of PlayerShopItem |  | Mặt hàng |

### Cấu trúc PlayerShopItem
Dữ liệu của một vật phẩm trong cửa hàng người chơi.

Độ dài: 20 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Khe vật phẩm |
| 1 | 12 | Binary |  | Dữ liệu mục |
| 16 | 4 | IntegerLittleEndian |  | Giá |

### Enum ActionKind
Loại hành động dẫn đến thông báo danh sách.

| Value | Name | Description |
|-------|------|-------------|
| 5 | ByRequest | Danh sách đã được yêu cầu. |
| 19 | UpdateAfterItemChange | Danh sách đã được thay đổi, ví dụ: bởi vì một mặt hàng đã được bán. |