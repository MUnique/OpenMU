# C3 32 - BuyItemFromNpcRequest (client gửi)

## Được gửi khi nào

Người chơi muốn mua một vật phẩm từ một thương gia NPC đã mở.

## Hành động phía server

Nếu người chơi có đủ tiền, vật phẩm sẽ được thêm vào kho và tiền sẽ bị xóa. Các tin nhắn tương ứng sẽ được gửi lại cho ứng dụng khách trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x32 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Khe vật phẩm; Khe vật phẩm (Cửa hàng NPC) |