# C1 96 - MutoNumberRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu thông tin về số Muto. Chưa sử dụng.

## Hành động phía server

Một phản hồi sẽ được gửi lại cho khách hàng kèm theo số Muto hiện tại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x96 | Tiêu đề gói - mã định danh loại gói |