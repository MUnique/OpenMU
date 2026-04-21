# C2 F4 02 - ServerListResponseOld (server gửi)

## Được gửi khi nào

Gói này được máy chủ gửi (bên dưới phần 1) sau khi khách hàng yêu cầu danh sách máy chủ hiện tại.

## Hành động phía client

Máy khách hiển thị các máy chủ có sẵn cùng với thông tin tải của chúng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0xF4 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 5 | 1 | Byte |  | Số lượng máy chủ |
| 6 | ServerLoadInfo.Length * | Array of ServerLoadInfo |  | Máy chủ |

### Cấu trúc ServerLoadInfo
Chứa id và tải của máy chủ.

Độ dài: 2 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | Id máy chủ |
| 1 | 1 | Byte |  | Phần trăm tải |