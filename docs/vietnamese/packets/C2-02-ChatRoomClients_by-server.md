# C2 02 - ChatRoomClients (server gửi)

## Được gửi khi nào

Gói này được máy chủ gửi sau khi một ứng dụng trò chuyện khác gửi tin nhắn đến phòng trò chuyện hiện tại.

## Hành động phía client

Khách hàng sẽ hiển thị thông báo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói |
| 6 | 1 | Byte |  | Số lượng khách hàng |
| 8 | ChatClient.Length * ClientCount | Array of ChatClient |  | Khách hàng |

### Cấu trúc ChatClient
Chứa chỉ mục và tên của ứng dụng trò chuyện được kết nối trong phòng.

Độ dài: 11 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Index |
| 1 | 10 | String |  | Name |