# C2 20 - ItemsDropped (server gửi)

## Được gửi khi nào

Vật phẩm rơi xuống đất.

## Hành động phía client

Khách hàng thêm các mục vào mặt đất.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC2 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 2 | Short |  | Tiêu đề gói - chiều dài của gói |
| 3 | 1 | Byte | 0x20 | Tiêu đề gói - mã định danh loại gói |
| 4 | 1 | Byte |  | số lượng vật phẩm |
| 5 | DroppedItem.Length * ItemCount | Array of DroppedItem |  | Mặt hàng |

### Cấu trúc DroppedItem
Chứa dữ liệu về một vật phẩm bị rơi.

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortBigEndian |  | Id |
| 0 << 7 | 1 bit | Boolean |  | IsFreshDrop; Nếu cờ này được đặt, mục sẽ được thêm vào bản đồ kèm theo hình ảnh động và âm thanh. Nếu không thì nó chỉ được thêm vào giống như nó đã có sẵn trước đó. |
| 2 | 1 | Byte |  | Vị tríX |
| 3 | 1 | Byte |  | Vị tríY |
| 4 |  | Binary |  | Dữ liệu mục |