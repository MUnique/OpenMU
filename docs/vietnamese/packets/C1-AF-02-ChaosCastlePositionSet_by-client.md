# C1 AF 02 - ChaosCastlePositionSet (client gửi)

## Được gửi khi nào

Khách hàng trò chơi nhận thấy rằng tọa độ của người chơi không còn ở trên mặt đất nữa. Nó yêu cầu thiết lập tọa độ được chỉ định.

## Hành động phía server

Máy chủ đặt người chơi ở tọa độ mới. Không được xử lý trên OpenMU.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAF | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 1 | Byte |  | Vị tríX |
| 5 | 1 | Byte |  | Vị tríY |