# C2 C0 - MessengerInitialization (server gửi)

## Được gửi khi nào

Sau khi vào game với một nhân vật.

## Hành động phía client



## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0xC0 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte |  | ThưĐếm |
| 5 | 1 | Byte |  | Số lượng thư tối đa |
| 6 | 1 | Byte |  | số lượng bạn bè |
| 7 | Friend.Length * FriendCount | Array of Friend |  | Bạn |

### Cấu trúc Friend
Cấu trúc chứa tên bạn bè và trạng thái trực tuyến.

Độ dài: 11 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |
| 10 | 1 | Byte |  | Id máy chủ; Id máy chủ mà người chơi hiện đang trực tuyến. 0xFF có nghĩa là ngoại tuyến. |