# C3 33 - SellItemToNpcRequest (client gửi)

## Được gửi khi nào

Người chơi muốn bán một vật phẩm trong kho của mình cho thương gia NPC đã mở.

## Hành động phía server

Vật phẩm được bán để lấy tiền cho NPC. Vật phẩm sẽ được xóa khỏi kho và tiền sẽ được thêm vào. Các tin nhắn tương ứng sẽ được gửi lại cho ứng dụng khách trò chơi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x33 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Khe vật phẩm; Khe vật phẩm (Kho đồ) |