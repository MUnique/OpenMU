# C1 AA 0B - DuelSpectatorList (server gửi)

## Được gửi khi nào

Khi một khán giả tham gia hoặc rời khỏi cuộc đấu tay đôi.

## Hành động phía client

Khách hàng cập nhật danh sách người xem.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 105 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0B | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Đếm |
| 5 | DuelSpectator.Length * | Array of DuelSpectator |  | Khán giả |

### Cấu trúc DuelSpectator
Cấu trúc lối vào phòng đấu tay đôi.

Độ dài: 10 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |