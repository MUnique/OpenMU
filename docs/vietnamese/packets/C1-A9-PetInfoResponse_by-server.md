# C1 A9 - PetInfoResponse (server gửi)

## Được gửi khi nào

Sau khi khách hàng gửi PetInfoRequest cho thú cưng (quạ đen, ngựa).

## Hành động phía client

Khách hàng hiển thị thông tin về thú cưng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 13 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xA9 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | ClientToServer.PetType |  | Thú cưng |
| 4 | 1 | ClientToServer.StorageType |  | Kho |
| 5 | 1 | Byte |  | Khe vật phẩm |
| 6 | 1 | Byte |  | Mức độ |
| 8 | 4 | IntegerLittleEndian |  | Kinh nghiệm |
| 12 | 1 | Byte |  | Sức khỏe |