# C1 87 - CraftingDialogCloseRequest (client gửi)

## Được gửi khi nào

Người chơi đóng hộp thoại được mở bằng cách tương tác với yêu tinh cỗ máy hỗn loạn.

## Hành động phía server

Máy chủ cập nhật trạng thái của người chơi tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x87 | Tiêu đề gói - mã định danh loại gói |