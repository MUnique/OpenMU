# C1 32 - ItemBought (server gửi)

## Được gửi khi nào

Yêu cầu mua vật phẩm từ người chơi hoặc npc đã thành công.

## Hành động phía client

Mặt hàng đã mua sẽ được thêm vào kho.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x32 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Khe hàng tồn kho |
| 4 |  | Binary |  | Dữ liệu mục |