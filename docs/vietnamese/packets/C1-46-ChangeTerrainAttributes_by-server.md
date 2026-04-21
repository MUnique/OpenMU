# C1 46 - ChangeTerrainAttributes (server gửi)

## Được gửi khi nào

Máy chủ muốn thay đổi các thuộc tính địa hình của bản đồ trong thời gian chạy.

## Hành động phía client

Máy khách cập nhật các thuộc tính địa hình về phía mình.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x46 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean | false | Type |
| 4 | 1 | TerrainAttributeType |  | Thuộc tính |
| 5 | 1 | Boolean |  | Xóa thuộc tính; Khi điều này đúng, thuộc tính sẽ bị xóa ở phía máy khách. Nếu sai thì thuộc tính sẽ được thêm vào. |
| 6 | 1 | Byte |  | Diện tíchĐếm |
| 7 | TerrainArea.Length * AreaCount | Array of TerrainArea |  | Khu vực |

### Cấu trúc TerrainArea
Xác định khu vực cần thay đổi.

Độ dài: 4 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Bắt đầuX |
| 1 | 1 | Byte |  | Bắt đầuY |
| 2 | 1 | Byte |  | EndX |
| 3 | 1 | Byte |  | CuốiY |

### Enum TerrainAttributeType
Xác định thuộc tính cần được đặt/bỏ đặt. Đó là một bảng liệt kê Flags.

| Value | Name | Description |
|-------|------|-------------|
| 1 | Safezone | Tọa độ là vùng an toàn. |
| 2 | Character | Tọa độ bị chiếm bởi một ký tự. |
| 4 | Blocked | Tọa độ bị chặn và một ký tự không thể truyền qua. |
| 8 | NoGround | Tọa độ bị chặn vì không có mặt bằng và nhân vật không thể vượt qua. |
| 16 | Water | Tọa độ bị nước chặn và nhân vật không thể vượt qua. |