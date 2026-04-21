# C1 2F - MoneyDroppedExtended (server gửi)

## Được gửi khi nào

Tiền rơi xuống đất.

## Hành động phía client

Khách hàng thêm tiền vào mặt đất.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x2F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Boolean |  | IsFreshDrop; Nếu cờ này được đặt, tiền sẽ được thêm vào bản đồ kèm theo hình ảnh động và âm thanh. Nếu không, nó chỉ được thêm vào giống như nó đã có sẵn trước đó. |
| 4 | 2 | ShortLittleEndian |  | Id |
| 6 | 1 | Byte |  | Vị tríX |
| 7 | 1 | Byte |  | Vị tríY |
| 8 | 4 | IntegerLittleEndian |  | Số lượng |