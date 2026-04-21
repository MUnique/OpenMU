# C1 87 - CraftingDialogClosed075 (server gửi)

## Được gửi khi nào

Sau khi người chơi yêu cầu đóng hộp thoại chế tạo, xác nhận này sẽ được gửi lại cho khách hàng.

## Hành động phía client

Ứng dụng khách trò chơi sẽ đóng hộp thoại chế tạo.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x87 | Tiêu đề gói - mã định danh loại gói |