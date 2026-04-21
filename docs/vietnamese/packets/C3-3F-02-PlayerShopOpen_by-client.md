# C3 3F 02 - PlayerShopOpen (client gửi)

## Được gửi khi nào

Người chơi muốn mở cửa hàng vật phẩm cá nhân của mình.

## Hành động phía server

Cửa hàng vật phẩm cá nhân được mở và những người chơi xung quanh được thông báo về nó, bao gồm cả người chơi của chính họ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 30 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x3F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 26 | String |  | Tên cửa hàng |