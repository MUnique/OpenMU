# C3 03 - ChecksumResponse (client gửi)

## Được gửi khi nào

Gói này được khách hàng gửi dưới dạng phản hồi cho yêu cầu có giá trị thách thức.

## Hành động phía server

Bởi máy chủ ban đầu, điều này được sử dụng để phát hiện một máy khách đã sửa đổi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x03 | Tiêu đề gói - mã định danh loại gói |
| 4 | 4 | IntegerLittleEndian |  | Tổng kiểm tra |