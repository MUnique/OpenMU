# C1 A9 - PetInfoRequest (client gửi)

## Được gửi khi nào

Người chơi di chuột qua một con vật cưng. Khách hàng gửi yêu cầu này để lấy thông tin (cấp độ, kinh nghiệm) của thú cưng (quạ đen, ngựa).

## Hành động phía server

Máy chủ gửi PetInfoResponse.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA9 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | PetType |  | Thú cưng |
| 4 | 1 | StorageType |  | Kho |
| 5 | 1 | Byte |  | Khe vật phẩm |

### Enum PetType
Mô tả loại vật nuôi.

| Value | Name | Description |
|-------|------|-------------|
| 0 | DarkRaven | Con quạ đen tối. |
| 1 | DarkHorse | Con ngựa đen thú cưng. |

### Enum StorageType
Mô tả loại lưu trữ.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Inventory | Kho đồ của người chơi. |
| 1 | Vault | Kho tiền của người chơi. |
| 2 | TradeOwn | Kho lưu trữ giao dịch riêng. |
| 3 | TradeOther | Kho lưu trữ giao dịch của người chơi khác. |
| 4 | Crafting | Kho lưu trữ thủ công của người chơi. |
| 5 | PersonalShop | Cửa hàng lưu trữ của người chơi khác. |
| 254 | InventoryPetSlot | Khe chứa đồ của thú cưng. Nó được sử dụng khi thú cưng lên cấp. |