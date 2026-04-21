# C1 F4 02 - ServerListRequestOld (client gửi)

## Được gửi khi nào

Gói này được khách hàng gửi (bên dưới phần 1) sau khi nó kết nối và nhận được tin nhắn 'Xin chào'.

## Hành động phía server

Máy chủ sẽ gửi lại ServerListResponseOld cho máy khách.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF4 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói phụ |