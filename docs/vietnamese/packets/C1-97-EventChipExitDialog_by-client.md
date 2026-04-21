# C1 97 - EventChipExitDialog (client gửi)

## Được gửi khi nào

Người chơi yêu cầu đóng hộp thoại chip sự kiện.

## Hành động phía server

Hộp thoại chip sự kiện sẽ đóng lại.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x97 | Tiêu đề gói - mã định danh loại gói |