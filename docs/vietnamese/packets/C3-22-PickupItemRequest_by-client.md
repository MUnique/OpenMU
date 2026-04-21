# C3 22 - PickupItemRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu nhặt một vật phẩm nằm trên mặt đất ở gần nhân vật của người chơi.

## Hành động phía server

Nếu người chơi được phép nhặt vật phẩm đó và là người chơi đầu tiên đã thử điều đó, thì người chơi sẽ cố gắng thêm vật phẩm đó vào kho. Máy chủ gửi phản hồi về kết quả của yêu cầu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x22 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Mã mặt hàng |