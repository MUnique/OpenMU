# C1 3F 06 - PlayerShopBuyResult (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu mua một vật phẩm trong shop của người chơi khác.

## Hành động phía client

Kết quả được hiển thị cho người chơi. Nếu thành công, mặt hàng sẽ được thêm vào kho.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 21 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x06 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | ResultKind |  | Kết quả |
| 5 | 2 | ShortBigEndian |  | ID người bán |
| 7 | 13 | Binary |  | Dữ liệu mục |
| 20 | 1 | Byte |  | Khe vật phẩm |

### Enum ResultKind
Loại kết quả.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Kết quả không xác định. |
| 1 | Success | Sản phẩm đã được mua thành công. |
| 2 | NotAvailable | Người bán không có sẵn. |
| 3 | ShopNotOpened | Người chơi được yêu cầu không có cửa hàng mở. |
| 4 | InTransaction | Người chơi được yêu cầu đã giao dịch với người chơi khác. |
| 5 | InvalidShopSlot | Vị trí vật phẩm được yêu cầu không hợp lệ. |
| 6 | NameMismatchOrPriceMissing | Trình phát được yêu cầu với id được chỉ định có tên khác hoặc thiếu giá. |
| 7 | LackOfMoney | Người chơi không có đủ tiền để mua vật phẩm từ người bán. |
| 8 | MoneyOverflowOrNotEnoughSpace | Người chơi bán không thể bán vật phẩm vì việc bán hàng sẽ làm tràn số tiền của anh ta trong kho. Một khả năng khác là hàng tồn kho của người mua không lấy được hàng. |
| 9 | ItemBlock | Người chơi được yêu cầu đã kích hoạt khối vật phẩm. |