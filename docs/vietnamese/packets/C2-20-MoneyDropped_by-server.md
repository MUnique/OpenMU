# C2 20 - MoneyDropped (server gửi)

## Được gửi khi nào

Tiền rơi xuống đất.

## Hành động phía client

Khách hàng thêm tiền vào mặt đất.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short | 21 | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x20 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte | 1 | số lượng vật phẩm |
| 5 | 2 | ShortBigEndian |  | Id |
| 5 << 7 | 1 bit | Boolean |  | IsFreshDrop; Nếu cờ này được đặt, tiền sẽ được thêm vào bản đồ kèm theo hình ảnh động và âm thanh. Nếu không thì nó chỉ được thêm vào giống như nó đã có sẵn trước đó. |
| 7 | 1 | Byte |  | Vị tríX |
| 8 | 1 | Byte |  | Vị tríY |
| 9 | 1 | Byte | 15 | TiềnSố |
| 10 | 4 | IntegerLittleEndian |  | Số lượng |
| 14 | 1 | Byte | 14 | Nhóm tiền |