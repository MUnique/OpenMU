# C3 0E 00 - Ping (client gửi)

## Được gửi khi nào

Gói này được khách hàng gửi cứ sau vài giây. Nó chứa "TickCount" hiện tại của hệ điều hành máy khách và tốc độ tấn công của nhân vật đã chọn.

## Hành động phía server

Bởi máy chủ ban đầu, điều này được sử dụng để phát hiện các vụ hack tốc độ.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 12 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x0E | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 4 | IntegerLittleEndian |  | Đếm đánh dấu |
| 8 | 2 | ShortLittleEndian |  | Tốc độ tấn công |