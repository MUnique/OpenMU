# C1 95 - EventChipRegistrationRequest (client gửi)

## Được gửi khi nào

Người chơi đăng ký một vật phẩm sự kiện tại NPC, thường là cung thủ vàng.

## Hành động phía server

Một phản hồi sẽ được gửi lại cho khách hàng cùng với số lượng chip sự kiện hiện tại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x95 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Type |
| 4 | 1 | Byte |  | Mục mục |