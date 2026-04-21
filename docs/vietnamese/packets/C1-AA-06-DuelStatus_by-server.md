# C1 AA 06 - DuelStatus (server gửi)

## Được gửi khi nào

Khi khách hàng yêu cầu danh sách các phòng đấu tay đôi hiện tại.

## Hành động phía client

Khách hàng hiển thị danh sách các phòng đấu tay đôi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 92 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x06 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | DuelRoomStatus.Length * | Array of DuelRoomStatus |  | Phòng |

### Cấu trúc DuelRoomStatus
Cấu trúc lối vào phòng đấu tay đôi.

Độ dài: 22 byte

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Tên người chơi1 |
| 10 | 10 | String |  | Tên người chơi2 |
| 20 | 1 | Boolean |  | Đấu tay đôiChạy |
| 21 | 1 | Boolean |  | Đấu tay đôiMở |