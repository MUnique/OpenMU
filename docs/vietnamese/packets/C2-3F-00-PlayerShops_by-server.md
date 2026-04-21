# C2 3F 00 - PlayerShops (server gửi)

## Được gửi khi nào

Sau khi người chơi vào phạm vi của người chơi có cửa hàng đã mở.

## Hành động phía client

Tiêu đề cửa hàng người chơi được hiển thị ở những người chơi được chỉ định.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |
| 5 | 1 | Byte |  | Đếm cửa hàng |
| 6 | PlayerShop.Length * ShopCount | Array of PlayerShop |  | Cửa hàng |

### Cấu trúc PlayerShop
Dữ liệu cửa hàng của người chơi.

Độ dài: 38 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id người chơi |
| 2 | 36 | String |  | Tên cửa hàng |