# C3 B1 01 - ServerChangeAuthentication (client gửi)

## Được gửi khi nào

Sau khi máy khách kết nối với máy chủ khác do thay đổi bản đồ.

## Hành động phía server

Người chơi xuất hiện trên máy chủ mới.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 69 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xB1 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x01 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 12 | Binary |  | Tài khoảnXor3 |
| 16 | 12 | Binary |  | Tên nhân vậtXor3 |
| 28 | 4 | IntegerLittleEndian |  | Mã xác thực1 |
| 32 | 4 | IntegerLittleEndian |  | Mã xác thực2 |
| 36 | 4 | IntegerLittleEndian |  | Mã xác thực3 |
| 40 | 4 | IntegerLittleEndian |  | Mã xác thực4 |
| 44 | 4 | IntegerLittleEndian |  | Đếm đánh dấu |
| 48 | 5 | Binary |  | Phiên bản khách hàng |
| 53 | 16 | Binary |  | Khách hàng nối tiếp |