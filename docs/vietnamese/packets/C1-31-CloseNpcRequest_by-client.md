# C1 31 - CloseNpcRequest (client gửi)

## Được gửi khi nào

Người chơi đóng hộp thoại được mở khi tương tác với NPC.

## Hành động phía server

Máy chủ cập nhật trạng thái của người chơi tương ứng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x31 | Tiêu đề gói - mã định danh loại gói |