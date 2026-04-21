# C1 32 FF - NpcItemBuyFailed (server gửi)

## Được gửi khi nào

Yêu cầu mua vật phẩm từ NPC không thành công.

## Hành động phía client

Khách hàng phản hồi lại. Nếu không có thông báo này, nó có thể bị kẹt.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x32 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0xFF | Tiêu đề gói - mã định danh loại gói phụ |